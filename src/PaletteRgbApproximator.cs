/* Copyright 2021 Intbeam
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

namespace SpriteConverter
{
    /// <summary>
    /// Approximates a color in the RGB color space
    /// These colors may not be perceptually similar so some
    /// Odd colors may appear
    /// </summary>
    public class PaletteRgbApproximator : IPaletteApproximator
    {
        private readonly Palette palette;
        public PaletteRgbApproximator(Palette palette)
        {
            this.palette = palette;
        }

        public int FindNearestColor(float r, float g, float b)
        {
            // pick a color at random. If something is wrong with the
            // delta it's immediately obvious what the problem is
            // if all the pixels are random
            
            int currentClosest = new Random().Next(0, palette.Count);
            // Get the delta to the current color
            var pDelta = GetDelta(
                (
                    palette[currentClosest].Red, 
                    palette[currentClosest].Green, 
                    palette[currentClosest].Blue
                ), 
                (
                    r, 
                    g, 
                    b
                )
            );

            // iterate through all the colors in the palette
            for (int i = 0; i < palette.Count; i++)
            {
                var p = palette[i];
                
                // get the delta for this one
                var nDelta = GetDelta((p.Red, p.Green, p.Blue), (r, g, b));

                // if this is closer than what we already have
                if(nDelta < pDelta)
                {
                    // assign it as current best match
                    pDelta = nDelta;
                    currentClosest = i;
                }
            }

            // return best match
            return currentClosest;
        }

        private float GetDelta((float, float, float) c1, (float, float, float) c2)
        {
            // Get the euclidian distance between the points
            return System.Numerics.Vector3.Distance(
                new System.Numerics.Vector3(c1.Item1, c1.Item2, c1.Item3), 
                new System.Numerics.Vector3(c2.Item1, c2.Item2, c2.Item3)
            );
        }

    }
}
