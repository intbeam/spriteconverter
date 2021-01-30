using System.Collections.Generic;

namespace SpriteConverter
{
    public struct RlePacket<T> : IRlePacket<T>
    {
        private readonly int count;
        private readonly T item;

        public RlePacket(T item, int count)
        {
            if (count <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(count), "Must be greater than zero");

            this.count = count;
            this.item = item;
        }

        public int Count => count;

        public T Item => item;

        public override string ToString()
        {
            if(item is byte b)
            {
                return $"r['{b.ToString("X2")}'x{count}]";
            }
            return $"r['{item}'x{count}]";
        }

        public IEnumerable<T> GetContent()
        {
            for (int i = 0; i < count; i++)
            {
                yield return item;
            }
        }

    }
}
