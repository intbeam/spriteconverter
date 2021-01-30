using System;
using System.Collections.Generic;
using System.Linq;

namespace SpriteConverter
{
    public interface IRleEncoder
    {
        IEnumerable<IRlePacket<T>> RleEncode<T>(Span<T> input, int windowSize, int minRepetition, int maxPacketSize);
        T[] RleDecode<T>(IEnumerable<IRlePacket<T>> input);
        IEnumerable<IRlePacket<T>> RleEncode<T>(Span<T> input, int minRepetition, int maxPacketSize);
    }

    public sealed class RleEncoder : IRleEncoder
    {
        private static void EnqueueRlePacket<T>(T item, int count, Queue<IRlePacket<T>> packets, int maxSize = int.MaxValue)
        {
            if (count == 0)
                return;

            if (count < maxSize)
            {
                packets.Enqueue(new RlePacket<T>(item, count));
                return;
            }

            var number = count / maxSize;
            var rest = count % maxSize;

            for (int i = 0; i < number; i++)
            {
                packets.Enqueue(new RlePacket<T>(item, maxSize));
            }

            if (rest > 0)
            {
                packets.Enqueue(new RlePacket<T>(item, rest));
            }

        }

        private static void EnqueueGeneralPacket<T>(Span<T> sequence, Queue<IRlePacket<T>> packets, int maxSize = int.MaxValue)
        {
            if (sequence.Length == 0)
                return;

            if (sequence.Length < maxSize)
            {
                packets.Enqueue(new GeneralPacket<T>(sequence.ToArray()));
                return;
            }

            var number = sequence.Length / maxSize;
            var rest = sequence.Length % maxSize;

            for (int i = 0; i < number; i++)
            {
                packets.Enqueue(new GeneralPacket<T>(sequence.Slice(number * maxSize, maxSize).ToArray()));
            }

            if (rest > 0)
            {
                packets.Enqueue(new GeneralPacket<T>(sequence.Slice(sequence.Length - rest).ToArray()));
            }
        }

        /// <summary>
        /// Encodes windows where each window should hold a specific number of total items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The sequence to encode</param>
        /// <param name="windowSize">The size of each window</param>
        /// <param name="minRepetition">Minimum number of equal items before encoded as RLE</param>
        /// <param name="maxPacketSize">Maximum number of elements in a packet</param>
        /// <returns></returns>
        public IEnumerable<IRlePacket<T>> RleEncode<T>(Span<T> input, int windowSize, int minRepetition, int maxPacketSize)
        {
            if (input.Length < windowSize || windowSize == 0)
            {
                return RleEncode(input, minRepetition, maxPacketSize);
            }

            var segments = new List<T[]>();
            var rest = input.Length % windowSize;
            var j = 0;

            for (int i = 0; i < input.Length / windowSize; i++)
            {                
                segments.Add(input.Slice(j, windowSize).ToArray());

                j += windowSize;
            }

            if(segments.Sum(n => n.Length) != input.Length)
            {
                throw new InvalidOperationException("Segments length differs from input");
            }

            if (rest > 0)
            {
                segments.Add(input.Slice(input.Length - rest).ToArray());
            }

            var packets = new List<IRlePacket<T>>(); ;

            foreach (var segment in segments)
            {
                var s = RleEncode<T>(segment, minRepetition, maxPacketSize);

                packets.AddRange(s);
            }

            return packets;


        }

        public T[] RleDecode<T>(IEnumerable<IRlePacket<T>> input)
        {
            var items = input.ToList();

            if (items.Count == 0)
                return Array.Empty<T>();

            var size = items.Sum(n => n.Count);

            var result = new T[size];

            int index = 0;
            foreach(var item in items)
            {
                if(item is RlePacket<T> rp)
                {
                    new Span<T>(result, index, rp.Count).Fill(rp.Item);
                }
                else
                {
                    new Span<T>(item.GetContent().ToArray())
                        .CopyTo(new Span<T>(result, index, item.Count));
                }

                index += item.Count;
            }

            return result;

        }

        public IEnumerable<IRlePacket<T>> RleEncode<T>(Span<T> input, int minRepetition, int maxPacketSize)
        {
            if (minRepetition < 1)
                throw new ArgumentOutOfRangeException(nameof(minRepetition), "Must be greater than zero");

            if (maxPacketSize == 0)
                maxPacketSize = int.MaxValue;

            if (maxPacketSize < 1)
                throw new ArgumentOutOfRangeException(nameof(maxPacketSize), "Must be greater than or equal to zero");

            var packets = new Queue<IRlePacket<T>>();
            var count = 0;
            bool splice = false;
                        
            List<T> buffer = new List<T>();

            for(int index = 0; index <= input.Length; index++)
            {
                if (index < input.Length)
                {
                    var current = input[index];

                    buffer.Add(current);

                    if(index < input.Length - 1)
                    {
                        var next = input[index + 1];

                        if (object.Equals(next, current))
                        {
                            count++;
                        }
                        else
                        {
                            if (count >= minRepetition)
                            {
                                splice = true;
                            }

                            count = 0;
                        }
                    }
                }
                else
                {
                    splice = true;
                }

                

                if(splice)
                {
                    splice = false;

                    if (!buffer.Any())
                        throw new InvalidOperationException("Buffer is empty");
                    
                    T lastChar = buffer.Last();

                    var endRepeat = buffer.Reverse<T>().TakeWhile(n => object.Equals(n, lastChar)).ToList();

                    if (endRepeat.Count > minRepetition)
                    {
                        buffer.RemoveRange(buffer.Count - endRepeat.Count, endRepeat.Count);
                    }
                    else
                    {
                        endRepeat.Clear();
                    }

                    if (buffer.Count > 0)
                    {
                        EnqueueGeneralPacket(buffer.ToArray(), packets, maxPacketSize);
                    }

                    if (endRepeat.Count > 0)
                    {
                        EnqueueRlePacket(lastChar, endRepeat.Count, packets, maxPacketSize);
                    }

                    buffer.Clear();

                    

                }

            }


            return packets;

        }
    }
}
