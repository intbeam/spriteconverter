using System.Diagnostics;

namespace SpriteConverter
{
    [DebuggerDisplay("[{Red},{Green},{Blue}]")]
    public struct PaletteEntry
    {
        /// <summary>
        /// Gets or sets red channel (0-1)
        /// </summary>
        public float Red { get; set; }
        /// <summary>
        /// Gets or sets green channel (0-1)
        /// </summary>
        public float Green { get; set; }

        /// <summary>
        /// Gets or sets blue channel (0-1)
        /// </summary>
        public float Blue { get; set; }
        
    }
}
