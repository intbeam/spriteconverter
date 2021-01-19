namespace SpriteConverter
{
    /// <summary>
    /// Generates palettes according to a descriptive name
    /// </summary>
    public interface IPaletteReader
    {
        /// <summary>
        /// Creates a palette described by the name parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Palette GetPalette(string name);
    }
}
