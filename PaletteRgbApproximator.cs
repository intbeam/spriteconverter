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
