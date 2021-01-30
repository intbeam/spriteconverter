# Sprite Converter

This project's intent is to be a very simple way to convert images to sprite sheets using predefined palettes to a file format that is less CPU intensive than more modern formates such as PNG or WebM.

Although it has not been tested yet, this program should theoretically be cross-platform.

## License

[MIT](https://opensource.org/licenses/MIT)

## Requirements

- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)

## How to build

```sh
dotnet build ./SpriteConverter.sln
```

## Usage
```sh
	dotnet ./spriteconverter.exe [--filename source] [--palette palettename] [--outfile format] [--colormapper mapper] [--omitpalette true/false] [--rle true/false] [--rlewindow n]
```

Also accepts a list of filenames (line separated) via stdin

### Parameters

#### --filename

The file to convert. If this parameter is passed, STDIN will be ignored

#### --palette

The palette to use. Default palette reader is an auto-generator and will create the palette on every run. It accepts EGA and a few RGB-based palettes with 6-color grayscale (including black).

Valid values are :

- ega (16 colors)
- Rgb685 (240 colors, default)
- Rgb666 (216 colors)
- Rgb565 (150 colors)
- Rgb232 (12 colors)

#### --outfile

Specifies the format of the output filename. It uses a string format with replacement where ´{0}´ gets replaced with the name of the source file without extension, ´{1}´ represents the extension and ´{2}´ is the parent directory name. Defaults to `{0}.tga`

#### --colormapper

Name of the color mapper. Defaults to `rgb`. This uses a linear approximation in RGB space to find nearest color in the palette. Not perceptively accurate.

#### --omitpalette

Whether the palette should be omitted from the output. Default is `false`. Setting this to `true` will prevent the image from being loaded in normal applications, but can be useful for saving space.

#### --rle

Enabling this option will compress the image using run-length encoding

#### --rlewindow

This is a positive integer which will segment the imagine for compression so that segment will be a specific size. For example if you have 16x16 sprites, it might make sense to have sums of RLE packets of 16 pixels to avoid wrapping packets

## Future improvements

- Support for more file types (bmp, ico, gif)
- A more perceptually accurate colormapper; LAB/HSV plus sRGB ICC profile.. Or maybe even with an ICC profile for IBM 5151/5153? If that exists?)
- Dithering
- Performance improvements