using System.Collections.Generic;

namespace SpriteConverter
{


    public interface IRlePacket<T>
    {
        public abstract int Count { get; }

        public abstract IEnumerable<T> GetContent();

    }
}
