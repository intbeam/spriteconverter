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

using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SpriteConverter;

[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("R: {Red}, G : {Green}, Blue : {Blue}")]
public readonly struct Rgb : IDistance<Rgb>, IRgbConvert<Rgb>
{
    private readonly float[] _rgb;

    public float Red => _rgb[0];
    public float Green => _rgb[1];
    public float Blue => _rgb[2];

    public Rgb()
    {
        _rgb = [0f, 0f, 0f];
    }

    public Rgb(float red, float green, float blue)
    {
        _rgb = [red, green, blue];
    }

    public Rgb(byte red, byte green, byte blue)
    {
        _rgb = [red / 255f, green / 255f, blue / 255f];
    }

    public float Max()
    {
        return _rgb.Max();
    }

    public float Min()
    {
        return _rgb.Min();
    }

    public static explicit operator Vector3(Rgb item)
    {
        return new Vector3(item.Red, item.Green, item.Blue);
    }

    public float Distance(Rgb other)
    {
        return Vector3.Distance((Vector3)this, (Vector3)other);
        
    }

    public static Rgb operator -(Rgb lhs, Rgb rhs)
    {
        return new Rgb(rhs.Red - lhs.Red, rhs.Green - lhs.Green, rhs.Blue - lhs.Blue);
    }
	
    public static Rgb operator +(Rgb lhs, Rgb rhs)
    {
        return new Rgb(rhs.Red - lhs.Red, rhs.Green - lhs.Green, rhs.Blue - lhs.Blue);
    }

    public static bool operator ==(Rgb lhs, Rgb rhs)
    {
        var n = rhs - lhs;
        return n._rgb.All(x => x == 0);
    }
	
    public static bool operator !=(Rgb lhs, Rgb rhs)
    {
        var n = rhs - lhs;
        return n._rgb.Any(x => x != 0);
    }

    public static Rgb FromRgb(Rgb color)
    {
        return color;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Rgb other)
        {
            return other == this;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (Red, Green, Blue).GetHashCode();
    }
}