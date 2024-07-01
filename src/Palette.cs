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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpriteConverter;
/// <summary>
/// Represents a set of colors
/// </summary>
public sealed class Palette : IEnumerable<Rgb>
{
    private readonly Rgb[] _entries;
    private static readonly Palette EmptyPalette = new (Enumerable.Empty<Rgb>());
    public static Palette Empty => EmptyPalette;

    public Palette(IEnumerable<Rgb> entries)
    {
        this._entries = entries.ToArray();
    }

    /// <summary>
    /// Gets the number of colors in this palette
    /// </summary>
    public int Count => _entries.Length;

    /// <summary>
    /// Gets the color entry at index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Rgb this[int index] => _entries[index];

    public IEnumerator<Rgb> GetEnumerator()
    {
        return (IEnumerator<Rgb>)_entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
