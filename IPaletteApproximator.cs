namespace SpriteConverter
{
    /// <summary>
    /// Attempts to approximate a color in a palette
    /// </summary>
    public interface IPaletteApproximator
    {
        /// <summary>
        /// Gets the index for the color that is the closest approximation to the color provided
        /// </summary>
        /// <param name="r">Red color between 0 and 1</param>
        /// <param name="g">Green color between 0 and 1</param>
        /// <param name="b">Blue color between 0 and 1</param>
        /// <returns>Index in the palette for the approximate color</returns>
        public int FindNearestColor(float r, float g, float b);
    }

}
