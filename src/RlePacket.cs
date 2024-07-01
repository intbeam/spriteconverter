/* Copyright 2024 Intbeam
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections.Generic;

namespace SpriteConverter;
public readonly struct RlePacket<T> : IRlePacket<T>
{

    public RlePacket(T item, int count)
    {
        if (count <= 0)
            throw new System.ArgumentOutOfRangeException(nameof(count), "Must be greater than zero");

        Count = count;
        Item = item;
    }

    public int Count { get; }

    public T Item { get; }

    public override string ToString()
    {
        if(Item is byte b)
        {
            return $"r['{b:X2}'x{Count}]";
        }
        return $"r['{Item}'x{Count}]";
    }

    public IEnumerable<T> GetContent()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return Item;
        }
    }

}
