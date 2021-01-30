using System.Collections.Generic;
using System.Linq;

namespace SpriteConverter
{
    public struct GeneralPacket<T> : IRlePacket<T>
    {
        private readonly T[] content;

        public GeneralPacket(T[] input)
        {
            if (input == null)
                throw new System.ArgumentNullException(nameof(input));

            if (input.Length == 0)
                throw new System.ArgumentException("Must have at least one element", nameof(input));

            content = input;
        }

        public GeneralPacket(IEnumerable<T> input) : this(input.ToArray())
        {
        }

        public int Count => content.Length;


        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb = sb.Append("g[\"");
            if (typeof(T) == typeof(byte))
            {
                var str = string.Join(" ", content.Cast<byte>().Select(n => n.ToString("X2")));
                sb.Append(str);
            }
            else
            {
                foreach (var item in content)
                {
                    if (item is byte b)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                    else
                        sb.Append(item);
                }
            }
            sb = sb.Append("\"]");

            return sb.ToString();
        }

        public IEnumerable<T> GetContent()
        {
            return content;
        }
    }
}
