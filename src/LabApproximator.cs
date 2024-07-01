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

using System;
using System.Diagnostics;

namespace SpriteConverter;

public class LabApproximator : BaseApproximator<LabApproximator.Lab>
{

	[DebuggerDisplay("X: {X}, Y: {Y}, Z : {Z}")]
	public readonly struct Xyz
	{
		public float X { get; }
		public float Y { get; }
		public float Z { get; }

		public Xyz(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	[DebuggerDisplay("L: {L}, A : {A}, B : {B}")]
	public readonly struct Lab : IDistance<Lab>, IRgbConvert<Lab>
	{
		public float L { get; }
		public float A { get; }
		public float B { get; }

		public Lab(float l, float a, float b)
		{
			L = l;
			A = a;
			B = b;
		}

		public float Distance(Lab other)
		{
			var lmean = (L + other.L) / 2.0f;
		    var c1 =  float.Sqrt(A*A + B*B);
		    var c2 =  float.Sqrt(other.A*other.A + other.B*other.B);
		    var cmean = (c1 + c2) / 2.0f;

		    var g =  ( 1 - float.Sqrt( float.Pow(cmean, 7) / (float.Pow(cmean, 7) + float.Pow(25, 7)) ) ) / 2; //ok
		    var a1Prime = A * (1 + g);
		    var a2Prime = other.A * (1 + g);

		    var c1Prime =  float.Sqrt(a1Prime*a1Prime + B*B);
		    var c2Prime =  float.Sqrt(a2Prime*a2Prime + other.B*other.B);
		    var cmeanprime = (c1Prime + c2Prime) / 2;

		    var h1Prime =  float.Atan2(B, a1Prime) + 2*float.Pi * (float.Atan2(B, a1Prime)<0 ? 1 : 0);
		    var h2Prime =  float.Atan2(other.B, a2Prime) + 2*float.Pi * (float.Atan2(other.B, a2Prime)<0 ? 1 : 0);
		    var hmeanprime =  ((float.Abs(h1Prime - h2Prime) > float.Pi) ? (h1Prime + h2Prime + 2*float.Pi) / 2 : (h1Prime + h2Prime) / 2);

		    var T =  1.0 - 0.17 * Math.Cos(hmeanprime - float.Pi/6.0) + 0.24 * float.Cos(2*hmeanprime) + 0.32 * float.Cos(3*hmeanprime + float.Pi/30) - 0.2 * float.Cos(4*hmeanprime - 21*float.Pi/60);

		    var deltahprime =  ((float.Abs(h1Prime - h2Prime) <= float.Pi) ? h2Prime - h1Prime : (h2Prime <= h1Prime) ? h2Prime - h1Prime + 2*float.Pi : h2Prime - h1Prime - 2*float.Pi);

		    double deltaLprime = (other.L - L);
		    var deltaCprime = c2Prime - c1Prime;
		    var deltaHprime =  2.0f * float.Sqrt(c1Prime*c2Prime) * float.Sin(deltahprime / 2.0f);
		    var sl =  1.0 + ( (0.015*(lmean - 50)*(lmean - 50)) / (float.Sqrt( 20 + (lmean - 50)*(lmean - 50) )) );
		    var sc =  1.0 + 0.045 * cmeanprime;
		    var sh =  1.0 + 0.015 * cmeanprime * T;

		    var deltaTheta =  (30 * float.Pi / 180) * float.Exp(-((180/float.Pi*hmeanprime-275)/25)*((180/float.Pi*hmeanprime-275)/25));
		    var rc =  (2 * float.Sqrt(float.Pow(cmeanprime, 7) / (float.Pow(cmeanprime, 7) + float.Pow(25, 7))));
		    var rt =  (-rc * float.Sin(2 * deltaTheta));

		    const float kl = 1;
		    const float kc = 1;
		    const float kh = 1;

		    var deltaE = (float)Math.Sqrt(
		            ((deltaLprime/(kl*sl)) * (deltaLprime/(kl*sl))) +
		            ((deltaCprime/(kc*sc)) * (deltaCprime/(kc*sc))) +
		            ((deltaHprime/(kh*sh)) * (deltaHprime/(kh*sh))) +
		            (rt * (deltaCprime/(kc*sc)) * (deltaHprime/(kh*sh)))
		            );

		    return deltaE;
		}

		public static Lab FromRgb(Rgb rgb)
		{
			return XyzToLab(RgbToXyz(rgb));
		}
		
		public static float XyzForm(float input)
		{
			if (input > 0.04045f)
			{
				input = float.Pow((input + 0.055f) / 1.055f, 2.4f);
				return input;
			}

			return input / 12.92f;
		}
		
		public static Xyz RgbToXyz(Rgb entry)
		{
			var r = XyzForm(entry.Red);
			var g = XyzForm(entry.Green);
			var b = XyzForm(entry.Blue);

			r *= 100f;
			g *= 100f;
			b *= 100f;

			return new Xyz(
				(r * 0.4124f + g * 0.3576f + b * 0.1805f),
				(r * 0.2126f + g * 0.7152f + b * 0.0722f),
				(r * 0.0193f + g * 0.1192f + b * 0.9505f)
			);
		}
		
		const float OneThird = 1f / 3f;
		const float Sixteenth = 16f / 116f;
		public static float LabForm(float input)
		{
			return input > 0.008856f
				? float.Pow(input, OneThird) 
				: (7.787f * input) + Sixteenth;
		}
    
		public static Lab XyzToLab(Xyz entry)
		{
			var x = entry.X / 95.0489f;
			var y = entry.Y / 100f;
			var z = entry.Z / 108.883f;



			x = LabForm(x);
			y = LabForm(y);
			z = LabForm(z);


			var l = (116f * y) - 16f;
			var a = 500f * (x - y);
			var b = 200f * (y - z);

			return new Lab(float.Max(0f, l), a, b);

		}
	}
	
    public LabApproximator(Palette palette) : base(palette)
    {
    }

    
    
}