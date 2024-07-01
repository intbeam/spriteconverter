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

public readonly struct Hsl : IDistance<Hsl>, IRgbConvert<Hsl>
{
    public float H { get; }
    public float S { get; }
    public float L { get; }

    public Hsl(float hue, float saturation, float lightness)
    {
        H = hue;
        S = saturation;
        L = lightness;
    }

    public float Distance(Hsl other)
    {
        return float.Sqrt(float.Pow(H - other.H, 2) + float.Pow(S - other.S, 2) + float.Pow(L - other.L, 2));
    }

    public static Hsl FromRgb(Rgb rgb)
    {
        var min = rgb.Min();
        var max = rgb.Max();
        var delta = max - min;

        var lightness = (max + min) / 2f;

        if (delta == 0f)
            return new Hsl(0f, lightness, 0f);

        var hue = 0f;
        float saturation;
        
        
        if (lightness < 0.5d) 
            saturation = delta / (max + min);
        else
            saturation = delta / (2f - max - min);

        var deltaR = (((max - rgb.Red) / 6f) + (delta / 2f)) / delta;
        var deltaG = (((max - rgb.Green) / 6f) + (delta / 2f)) / delta;
        var deltaB = (((max - rgb.Blue) / 6f) + (delta / 2f)) / delta;

        if (rgb.Red >= max) 
            hue = deltaB - deltaG;
        else if (rgb.Green >= max) 
            hue = (1f / 3f) + deltaR - deltaB;
        else if (rgb.Blue >= max) 
            hue = (2f / 3f) + deltaG - deltaR;

        if (hue < 0) 
            hue += 1;
        else if (hue > 1) 
            hue -= 1;

        return new Hsl(hue, lightness, saturation);

        
    }
		
}