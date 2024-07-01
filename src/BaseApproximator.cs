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

namespace SpriteConverter;

/// <summary>
/// Base approximator given a type that implements <see cref="IDistance{T}"/> and <see cref="IRgbConvert{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseApproximator<T> : IPaletteApproximator where T : struct, IDistance<T>, IRgbConvert<T>
{
    private readonly Palette _palette;

    public BaseApproximator(Palette palette)
    {
        _palette = palette;
    }

    protected virtual T[] InitializeTable()
    {
        var values = new T[_palette.Count];

        for(var i = 0; i < _palette.Count; i++)
        {
            values[i] = T.FromRgb(_palette[i]);
        }

        return values;
    }
	
    public virtual int FindNearestColor(Rgb source, out Rgb error)
    {
        var vec = T.FromRgb(source);

        var delta = double.MaxValue;
        var closest = -1;
        for (int i = 0; i < this._palette.Count; i++)
        {
            var distance = vec.Distance(T.FromRgb(_palette[i]));
				
            if (distance < delta)
            {
                closest = i;
                delta = distance;
            }
			
        }

        var res = _palette[closest];
        var palrgb = new Rgb(res.Red, res.Green, res.Blue);
        error = palrgb - source;
		
        return closest;
		
    }
	
	
}