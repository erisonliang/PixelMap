using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Integration;
using PixelMapSharp;

namespace PixelMapTest
{
    class Program
    {
        private static int width = 512;
        private static int height = 512;
        private static int w = 1;
        private static int h = 1;

        private static PixelMap input;
        private static PixelMap output;
        static void Main(string[] args)
        {
            output = new PixelMap(width, height);
            input = new PixelMap(new Bitmap("input.jpg"));

            var vectorField = buildField(new Bitmap("normals.png"));

            for (int x = 0; x < width / w; x++)
            {
                for (int y = 0; y < height / h; y++)
                {
                    var vector = vectorField[x, y];

                    if(vector.Magnitude<0.3)
                        vectorField[x, y] = Vector2.FromTheta(vector.Theta,vector.Magnitude);

                    double val = (vector.Theta / (Math.PI * 2));

                    Color c = lerpC(Color.OrangeRed, Color.DodgerBlue, fit(vector.Magnitude));


                    output.DrawLine(new Point(x * w, y * h), new Point(x * w + (int)(vector.X * 2), y * h + (int)(vector.Y * 2)), c);
                }
            }

            for (int i = 0; i < 100; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Vector2 location = new Vector2(x, y);
                        fall(location, vectorField,i);
                    }

                }

                output.Bitmap.Save("frames\\"+i+".png");
            }

            output.Bitmap.Save("output.png");

        }

        static Vector2[,] buildField(Bitmap input)
        {
            Vector2[,] vectorField = new Vector2[width / w, height / h];
            for (int x = 0; x < width / w; x++)
            {
                for (int y = 0; y < height / h; y++)
                {
                    var c = input.GetPixel(x, y);

                    vectorField[x, y] = new Vector2(c.R / 255d - 0.5d, c.G / 255d - 0.5d);
                }
            }
            return vectorField;
        }

        static Random r = new Random();
        static void fall(Vector2 location, Vector2[,] field,int depth)
        {
            Vector2 lastLocation = new Vector2();
            Vector2 velocity = new Vector2(0, 0);

            const double friction = 0.8;
            Color c = input[(int)location.X, (int)location.Y];

            for (int i = 0; i < depth; i++)
            {
                lastLocation = location;
                location.X += velocity.X;
                location.Y += velocity.Y;

                if (output.Inside(new Point((int)location.X, (int)location.Y)))
                {
                    var force = getForce(field, location);


                    velocity.X += force.X * 8;
                    velocity.X *= friction;
                    velocity.Y += force.Y * 8;
                    velocity.Y *= friction;

                    output.DrawLine(new Point((int)location.X, (int)location.Y), new Point((int)lastLocation.X, (int)lastLocation.Y), c);
                }

            }


        }
        static double lerp(double v0, double v1, double t)
        {
            return v0 + t * (v1 - v0);
        }

        static Color lerpC(Color a, Color b, double t)
        {
            return Color.FromArgb((byte)lerp(a.R, b.R, t), (byte)lerp(a.G, b.G, t), (byte)lerp(a.B, b.B, t));
        }

        static Vector2 getForce(Vector2[,] field, Vector2 location)
        {
            if (location.X < 0 || location.Y < 0 || (int)(location.X / w) + 1 >= field.GetLength(0) || (int)(location.Y / h) + 1 >= field.GetLength(1))
                return new Vector2(0, 0);
            Vector2 a = field[(int)(location.X / w), (int)(location.Y / h)];
            Vector2 b = field[(int)(location.X / w) + 1, (int)(location.Y / h)];
            Vector2 c = field[(int)(location.X / w), (int)(location.Y / h) + 1];
            Vector2 d = field[(int)(location.X / w) + 1, (int)(location.Y / h) + 1];

            Vector2 lA = new Vector2((int)(location.X / w) * w, (int)(location.Y / h) * h);

            double x = bilerp(location.X - lA.X, location.Y - lA.Y, a.X, b.X, c.X, d.X);
            double y = bilerp(location.X - lA.X, location.Y - lA.Y, a.X, b.Y, c.Y, d.Y);

            return new Vector2(x, y);
        }

        static double bilerp(double x, double y, double a, double b, double c, double d)
        {
            x /= w;
            y /= h;
            return a * (1 - x) * (1 - y) + b * (1 - y) * (x) + c * (1 - x) * (y) + d * (x) * (y);
        }

        static double fit(double a)
        {
            return 1/(1 + a);
        }
    }



}
