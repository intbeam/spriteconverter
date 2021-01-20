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
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace SpriteConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["palette"] = StandardPaletteReader.PaletteRgb685,  // The palette to use
                    ["outfile"] = "{0}.tga",    // format of output file
                    ["colormapper"] = "rgb",    // what color mapping strategy to be used. Default RGB
                    ["filename"] = "",    // if input filename is null, the standard input stream will be read
                    ["omitpalette"] = "false", // setting this to false will prevent it from being opened in normal applications
                    ["format"] = "auto"
                })
                .AddJsonFile("SpriteConverter.json", true, false)
                .AddCommandLine(args, new Dictionary<string, string>
                {
                    ["--palette"] = "palette",
                    ["--outfile"] = "outfile",
                    ["--colormapper"] = "colormapper",
                    ["--filename"] = "filename",
                    ["--omitpalette"] = "omitpalette",
                    ["--format"] = "format"
                }
                );

            var config = configurationBuilder.Build();

            var paletteGenerator = new StandardPaletteReader();
            var palette = paletteGenerator.GetPalette(config["palette"]);
            var paletteApproximator = new PaletteRgbApproximator(palette);

            List<string> files = new List<string>();

            if(string.IsNullOrWhiteSpace(config["filename"]))
            {
                using var stream = System.Console.OpenStandardInput();
                using var streamReader = new System.IO.StreamReader(stream, System.Console.InputEncoding);

                string? line;
                while((line = streamReader.ReadLine()) != null)
                {
                    files.Add(line);
                }
            }
            else
            {
                files.Add(config["filename"]);
            }
            
            foreach(var file in files)
            {
                try
                {
                    // Get info for file so we can use this information the the out format
                    var fileInfo = new FileInfo(file);

                    if (!fileInfo.Exists)
                        throw new FileNotFoundException("Could not find source file", file);
                    // we will select a formatwriter based on extension
                    IFormatWriter? format = null;
                    // format outfile
                    var outFile = string.Format(config["outfile"], Path.GetFileNameWithoutExtension(fileInfo.Name), fileInfo.Extension, fileInfo.DirectoryName);

                    var outFileInfo = new FileInfo(outFile);

                    var formatName = "tga";

                    // Replace with factory when needed
                    if("auto".Equals(config["format"], StringComparison.OrdinalIgnoreCase))
                    {
                        // detect based on file extension
                        if (new[] { ".tga", ".icb", ".vda", ".vst" }.Any(n => n.Equals(outFileInfo.Extension, StringComparison.OrdinalIgnoreCase)))
                            formatName = "tga";
                    }
                    else
                    {
                        formatName = config["format"];
                    }

                    if(formatName == "tga")
                    { 
                        format = new TgaSpriteWriter(paletteApproximator, new TgaSpriteWriterOptions { WritePalette = "false".Equals(config["omitpalette"], System.StringComparison.OrdinalIgnoreCase) });
                    }
                    else
                    {
                        throw new System.NotImplementedException($"Extension {fileInfo.Extension} is not supported");
                    }

                    Console.Write($"Writing {outFileInfo.Name}...");
                    using (var bitmap = Image.FromFile(file))
                    using (var outStream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        format.Save(bitmap, outStream, new SpriteMetadata { Palette = palette });
                    }
                    Console.Write("Done.");
                    if(outFileInfo.Name.Length > 11)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(" Warning: this filename is longer than 11 characters!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine();
                }
                catch(FileNotFoundException ex)
                {
                    Console.WriteLine($"The file \"{ex.FileName}\" could not be found");
                }
                catch(DirectoryNotFoundException)
                {
                    Console.WriteLine($"The directory could not be found");
                }
                catch(FormatException)
                {
                    Console.WriteLine("The provided format for the output filename is invalid");
                    break; // this will happen to all the files
                }
                catch(OutOfMemoryException ex)
                {
                    Console.WriteLine("The system ran out of memory : " + ex.Message);
                    break;
                }
            }

        }
    }
}
