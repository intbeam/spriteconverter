using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace SpriteConverter
{
    /// <summary>
    /// Converts an image to the appropriate format
    /// </summary>
    public interface IFormatWriter
    {
        Task Save(Image bitmap, Stream output, SpriteMetadata metadata);
    }
}
