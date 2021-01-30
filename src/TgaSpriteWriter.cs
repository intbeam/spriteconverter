/* Copyright 2021 Intbeam
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SpriteConverter
{
    public sealed class TgaSpriteWriter : IFormatWriter
    {
        private readonly IPaletteApproximator paletteApproximator;
        private readonly TgaSpriteWriterOptions options;
        private const string TgaSignature = "TRUEVISION-XFILE.\0";
        private const int MaxRlePacketSize = 127;
        private const int MinRepetition = 2;
        private readonly IRleEncoder? rleEncoder;

        /// <summary>
        /// Creates a new Targa Sprite Writer
        /// </summary>
        /// <param name="paletteApproximator">The type of approximation to use in order to find closest colors</param>
        /// <param name="options">Write options</param>
        public TgaSpriteWriter(IPaletteApproximator paletteApproximator, TgaSpriteWriterOptions? options = default, IRleEncoder? rleEncoder = default)
        {
            this.options = options ?? new TgaSpriteWriterOptions();
            this.paletteApproximator = paletteApproximator;
            this.rleEncoder = rleEncoder;
        }

        public Task Save(Image sourceImage, Stream output, SpriteMetadata metadata)
        {
            // We need a binary writer and a bitmap wrapper for the source so we can inspect the image data
            using var writer = new BinaryWriter(output, System.Text.Encoding.ASCII, true);
            using var source = new Bitmap(sourceImage);

            if(options.WritePalette && metadata.Palette == null)
            {
                throw new ArgumentNullException(nameof(metadata.Palette), "Palette cannot be null if WritePalette is enabled");
            }

            if(options.RleEncode && rleEncoder == null)
            {
                throw new ArgumentException("Rle not supported in this configuration");
            }

            var header = new TgaHeader
            {
                // ImageId can put some sort of indentifier on the image..
                // A tag or something.. We won't be needing it.
                // This is essentially how many bytes the ImageId field takes
                // so 0 means no ImageId
                ImageId = 0,
                // If we're writing the palette, set it to indexed, otherwise nay
                ColorMapType = options.WritePalette ? ColorMapType.Indexed : ColorMapType.None, 
                // we will only be working with indexed palettes.. or will we?
                ImageType = options.RleEncode ? ImageType.RleIndexed : ImageType.Indexed, 
                // this is an offset if we we're only using colors after a certain index
                ColorMapFirstIndex = 0, 
                ColorMapLength = options.WritePalette ? (ushort)metadata.Palette.Count: ushort.MinValue, // number of colors in the palette
                // we will be using 24 bit colors for the palette (even though VGA is just 15 bit.. We'll just divide by four and cross our fingers)
                ColorMapBitsPerEntry = 24, 
                // This is a weird thing which positions the image according to the screen
                OriginX = 0, 
                OriginY = 0,
                // Image dimension in pixels
                ImageWidth = (ushort)source.Width, 
                ImageHeight = (ushort)source.Height, 
                // bits per pixel in the image data. Since we're using a 8 bit palette, this needs to be 8 as well
                BitsPerPixel = 8, 
                // TGA (and BMP) defaults to bottom to top scanline ordering, we want it from top to bottom
                ImageDescriptor = (byte)(ImageDescriptor.ImageOriginTopBottom)
            };

            // Create the palette data
            var paletteData = CreatePaletteData(metadata.Palette);
            // Create the image data
            var imageData = CreateImageData(source, header);
            
            // Start by writing the header we have prepared
            WriteHeader(writer, header);
            
            // Write the palette if we are going to
            if (options.WritePalette)
            {
                WritePaletteData(writer, paletteData);
            }

            // Write the image data
            WriteImageData(writer, imageData);

            // Write the footer
            // This is actually important because if this is not
            // done correctly, anything trying to load this file
            // should fail per the TGA specification
            WriteFooter(writer);

            // Haven't made this async yet
            return Task.CompletedTask;

        }

        /// <summary>
        /// Create a byte array containing the palette in 24-bit BGR order
        /// </summary>
        /// <param name="palette"></param>
        /// <returns></returns>
        private byte[] CreatePaletteData(Palette palette)
        {
            byte[] paletteData = new byte[palette.Count * 3];

            using (var memoryStream = new MemoryStream(paletteData, true))
            using (var pwriter = new BinaryWriter(memoryStream))
            {
                for (int i = 0; i < palette.Count; i++)
                {
                    var c = palette[i];
                    pwriter.Write((byte)Math.Floor(c.Blue * byte.MaxValue));
                    pwriter.Write((byte)Math.Floor(c.Green * byte.MaxValue));
                    pwriter.Write((byte)Math.Floor(c.Red * byte.MaxValue));

                }
            }

            return paletteData;
        }

        /// <summary>
        /// Creates an array containing the actual image data
        /// </summary>
        /// <param name="source"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private byte[] CreateImageData(Bitmap source, TgaHeader header)
        {
            int imageSize = header.ImageWidth * header.ImageHeight * (header.BitsPerPixel / 8);
            byte[] imageData = new byte[imageSize];
            // Go through each scanline
            for (int y = 0; y < header.ImageHeight; y++)
            {
                // Go through each column
                for (int x = 0; x < header.ImageWidth; x++)
                {
                    // Get the color at coordinate
                    var c = source.GetPixel(x, y);
                    int index;

                    // if alpha channel is zero, set it to 0 (let's revise this later)
                    if (c.A == 0)
                    {
                        index = 0;
                    }
                    else
                    {
                        // Get the index for the closest color in the palette
                        index = paletteApproximator.FindNearestColor(c.R / 255f, c.G / 255f, c.B / 255f);
                    }
                    // Set the data in the byte array to the value
                    imageData[y * header.ImageWidth + x] = (byte)index;
                }
            }

            return imageData;
        }

        private void WritePaletteData(BinaryWriter writer, byte[] paletteData)
        {
            writer.Write(paletteData);
        }

        private void WriteImageData(BinaryWriter writer, byte[] imageData)
        {
            if (this.options.RleEncode)
            {
                if (rleEncoder == null)
                    throw new InvalidOperationException("Rle encoder not set");

                var result = rleEncoder.RleEncode<byte>(imageData, options.RleWindowSize, MinRepetition, MaxRlePacketSize).ToList();
                


                if (result.Sum(n => n.Count) != imageData.Length)
                {
                    throw new InvalidOperationException("Encoded data corrupted");
                }
                

                foreach(var item in result)
                {
                    if (item is GeneralPacket<byte> gp)
                    {
                        byte packetHeader = (byte)(gp.Count - 1);
                        writer.Write(packetHeader);
                        writer.Write(item.GetContent().ToArray());
                    }
                    else if(item is RlePacket<byte> rp)
                    {
                        byte packetHeader = (byte)(0b10000000 | (rp.Count - 1));
                        writer.Write(packetHeader);
                        writer.Write(rp.Item);
                    }
                }

            }
            else
            {
                writer.Write(imageData);
            }
        }

        private void WriteFooter(BinaryWriter writer)
        {
            // Extension area offset
            writer.Write((UInt32)0);
            // Developer area offset
            writer.Write((UInt32)0);
            // Signature
            writer.Write(System.Text.Encoding.ASCII.GetBytes(TgaSignature));
        }

        private void WriteHeader(BinaryWriter writer, TgaHeader header)
        {
            writer.Write(header.ImageId);
            writer.Write((byte)header.ColorMapType);
            writer.Write((byte)header.ImageType);
            writer.Write(header.ColorMapFirstIndex);
            writer.Write(header.ColorMapLength);
            writer.Write(header.ColorMapBitsPerEntry);
            writer.Write(header.OriginX);
            writer.Write(header.OriginY);
            writer.Write(header.ImageWidth);
            writer.Write(header.ImageHeight);
            writer.Write(header.BitsPerPixel);
            writer.Write(header.ImageDescriptor);
        }

        [Flags]
        private enum ImageDescriptor
        {
            // Alpha tells how many bits in a true-color image are for alpha per channel
            Alpha0 =                0b00000001,
            Alpha1 =                0b00000010,
            Alpha2 =                0b00000100,
            Alpha3 =                0b00001000,
            // Mirrored
            ImageOriginaRightLeft = 0b00010000,
            // For some reason many bitmap formats
            // store the image upside down by default
            // this is to put it right
            ImageOriginTopBottom =  0b00100000,
            // two most significant bits are unused and must be zero for "future use"
        }

        private struct TgaHeader
        {
            /// <summary>
            /// When greater than zero, this specifies the length of the ImageId field
            /// </summary>
            public byte ImageId;

            /// <summary>
            /// True color or indexed
            /// </summary>
            public ColorMapType ColorMapType;

            /// <summary>
            /// Color type, run-length encoding
            /// </summary>
            public ImageType ImageType;

            /// <summary>
            /// First index used in the color map. Essentially an offset
            /// </summary>
            public ushort ColorMapFirstIndex;
            /// <summary>
            /// Number of colors in the palette
            /// </summary>
            public ushort ColorMapLength;
            /// <summary>
            /// Bits per color in palette
            /// </summary>
            public byte ColorMapBitsPerEntry;

            /// <summary>
            /// Origin x coordinate. This is the left-most part of the TARGA screen display (heh)
            /// </summary>
            public ushort OriginX;
            /// <summary>
            /// Origin y coordinate. This is the bottom-most part of the screen
            /// </summary>
            public ushort OriginY;
            /// <summary>
            /// Image width in pixels
            /// </summary>
            public ushort ImageWidth;
            /// <summary>
            /// Image height in pixels
            /// </summary>
            public ushort ImageHeight;
            /// <summary>
            /// Bits per pixel (8 for indexed)
            /// </summary>
            public byte BitsPerPixel;
            /// <summary>
            /// 
            /// </summary>
            public byte ImageDescriptor;
        }

        private enum ColorMapType : byte
        {
            None = 0x0,
            Indexed = 0x1,
        }

        private enum ImageType : byte
        {
            None = 0x00, // Yes, a TGA image is not required to contain an image
            Indexed = 0x01, // Indexed from color map
            TrueColor = 0x02, // True color RGB
            Bitmapped = 0x03, // 1-bit Black and white
            RleIndexed = 0x09, // Run-length encoded indexed
            RleTrueColor = 0x0a, // Run-length encoded true color
            RleBitmapped = 0x0b // Run-length encoded bitmap
        }
    }
}
