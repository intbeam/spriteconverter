# Sprite Converter

This project's intent is to be a very simple way to convert images to sprite sheets using predefined palettes to a file format that is less CPU intensive than more modern formates such as PNG or WebM.

## License

[MIT](https://opensource.org/licenses/MIT)

## Usage
```sh
	dotnet ./spriteconverter.exe [--filename source] [--palette palettename] [--outfile format] [--colormapper mapper] [--writepalette true/false]
```

Also accepts a list of filenames (line separated) via stdin like so :

```sh
echo "testfile.jpg" | dotnet ./spriteconverter.exe
```