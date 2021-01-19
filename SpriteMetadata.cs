namespace SpriteConverter
{
    /// <summary>
    /// Data relating to the storage of the sprite
    /// </summary>
    public class SpriteMetadata
    {
        public SpriteMetadata()
        {
        }

        public Palette Palette { get; set; } = Palette.Empty;
    }
}
