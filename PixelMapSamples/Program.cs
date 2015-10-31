using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelMapSharp;

namespace PixelMapSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Quickly load a PixelMap through a Bitmap
            PixelMap map = new PixelMap(new Bitmap("Lenna.png"));

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    //Sample a pixel
                    Pixel pixel = map[x, y];

                    //Create a hue value
                    double value = ((double)x / map.Width) * 360d;

                    //Set the hue value to our sample
                    pixel.Hue = value;

                    //Return our sample to the PixelMap
                    map[x, y] = pixel;
                }
            }

            //Save the PixelMap through a Bitmap
            map.GetBitmap().Save("output.png");
        }
    }
}
