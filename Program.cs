using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
                    ["writepalette"] = "true" // setting this to false will prevent it from being opened in normal applications
                })
                .AddJsonFile("SpriteConverter.json", true, false)
                .AddCommandLine(args, new Dictionary<string, string>
                {
                    ["--palette"] = "palette",
                    ["--outfile"] = "outfile",
                    ["--colormapper"] = "colormapper",
                    ["--filename"] = "filename",
                    ["--writepalette"] = "writepalette"
                }
                );

            var config = configurationBuilder.Build();

            var paletteGenerator = new StandardPaletteReader();
            var palette = paletteGenerator.GetPalette(config["palette"]);
            var paletteApproximator = new PaletteRgbApproximator(palette);

            List<string> files = new List<string>();

            if(string.IsNullOrWhiteSpace(config["filename"]))
            {
                using (var stream = System.Console.OpenStandardInput())
                using(var streamReader = new System.IO.StreamReader(stream, System.Console.InputEncoding))
                {
                    string? line;
                    while((line = streamReader.ReadLine()) != null)
                    {
                        files.Add(line);
                    }
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
                    var fileInfo = new FileInfo(file);
                    IFormatWriter? format = null;

                    var outFile = string.Format(config["outfile"], Path.GetFileNameWithoutExtension(fileInfo.Name), fileInfo.Extension, fileInfo.DirectoryName);

                    var outFileInfo = new FileInfo(outFile);

                    if (outFileInfo.Extension.Equals(".tga", System.StringComparison.OrdinalIgnoreCase))
                    {
                        format = new TgaSpriteWriter(paletteApproximator, new TgaSpriteWriterOptions { WritePalette = "true".Equals(config["writepalette"], System.StringComparison.OrdinalIgnoreCase) });
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
                    Console.WriteLine($"The file {ex.FileName} could not be found");
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
                }
            }

        }
    }
}
