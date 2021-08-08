using System.IO;
using Xamarin.Forms;
using SkiaSharp;

namespace WLED.Utilities
{
    class ImageUtils
    {
        public static Xamarin.Forms.Color GetAverageColor(Stream stream)
        {
            SKBitmap bmp = SKBitmap.Decode(stream);
            

            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    SKColor clr = bmp.GetPixel(x, y);

                    r += clr.Red;
                    g += clr.Green;
                    b += clr.Blue;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Xamarin.Forms.Color.FromRgb(r, g, b);
        }
    }
}
