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

using System.Collections.Generic;
using System.Linq;

namespace SpriteConverter
{
    /// <summary>
    /// Default palette reader. Creates palettes based on channel resolution. Also EGA.
    /// </summary>
    public class StandardPaletteReader : IPaletteReader
    {
        
        /// <summary>
        /// Creates a palette based on resolution. Will also add 6 gradients of grayscale.
        /// </summary>
        /// <param name="rResolution"></param>
        /// <param name="gResolution"></param>
        /// <param name="bResolution"></param>
        /// <returns></returns>
        protected virtual Palette CreateRgbPalette(int rResolution, int gResolution, int bResolution)
        {
            var grayshades = 6;
            var colors = rResolution * gResolution * bResolution;
            var palette = new PaletteEntry[colors + (grayshades + 1)];

            var index = 0;
            // Go through each channel resolution
            for(int r = 0; r < rResolution;r++)
            {
                for(int g = 0; g < gResolution; g++)
                {
                    for(int b = 0; b < bResolution; b++)
                    {
                        // Set the color, dividing it by max to get a floating point value between 0 and 1
                        palette[index++] = new PaletteEntry { 
                            Red = r / (float)(rResolution - 1), 
                            Green = g / (float)(gResolution - 1), 
                            Blue = b / (float)(bResolution - 1) 
                        };
                    }

                }
            }
            // Same general process for grayscale
            for(int bw = 0; bw < grayshades; bw++)
            {
                var v = (float)bw / (float)(grayshades - 1);
                palette[index++] = new PaletteEntry
                {
                    Red = v,
                    Green = v,
                    Blue = v
                };
            }

            return new Palette(palette);
        }

        // Some known palettes stored in the dictionary
        public const string PaletteRgb685 = "Rgb685";
        public const string PaletteRgb565 = "Rgb565";
        public const string PaletteRgb232 = "Rgb232";
        public const string PaletteEga = "ega";

        /// <summary>
        /// Known combinations
        /// </summary>
        private Dictionary<string, (int,int,int)> rgbPalettes = new Dictionary<string, (int,int,int)> {
            ["Rgb565"] = (5, 6, 5), // 150 color palette
            ["Rgb685"] = (6, 8, 5), // 240 color palette
            ["Rgb232"] = (2, 3, 2)  // 12 color palette
        };

        /// <summary>
        /// Creates an EGA palette
        /// </summary>
        /// <returns></returns>
        private Palette CreateEgaPalette()
        {
            return new Palette(Enumerable.Range(0, 16).Select(i => new PaletteEntry
            {
                // i represents the index in the ega palette
                Red =   85 * (((i >> 1) & 2) | (i >> 5) & 1) / 255f,
                Green = 85 * ((i & 2) | (i >> 4) & 1) / 255f,
                Blue =  85 * (((i << 1) & 2) | (i >> 3) & 1) / 255f
            }));
        }

        public Palette GetPalette(string name)
        {
            if(name == "ega")
            {
                return CreateEgaPalette();
            }

            if (!rgbPalettes.ContainsKey(name))
                throw new KeyNotFoundException("Unknown palette");

            (var r, var g, var b) = rgbPalettes[name];

            return CreateRgbPalette(r, g, b);
        }
    }
}
