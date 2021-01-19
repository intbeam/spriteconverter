using System.Collections.Generic;
using System.Linq;

namespace SpriteConverter
{
    /// <summary>
    /// Represents a set of colors
    /// </summary>
    public sealed class Palette
    {
        private readonly PaletteEntry[] entries;
        private static readonly Palette emptyPalette = new Palette(Enumerable.Empty<PaletteEntry>());
        public static Palette Empty { get => emptyPalette; }

        public Palette(IEnumerable<PaletteEntry> entries)
        {
            this.entries = entries.ToArray();
        }

        /// <summary>
        /// Gets the number of colors in this palette
        /// </summary>
        public int Count { get => entries.Length; }
        /// <summary>
        /// Gets the color entry at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PaletteEntry this[int index]
        {
            get => entries[index];
        }
    }
}
