using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PixelMapSharp;
using Svg;

namespace PixelMapForm
{
    public partial class Form1 : Form
    {
        private SvgDocument document;
        public Form1()
        {
            InitializeComponent();

            document = SvgDocument.Open("input.svg");

            

            Timer t = new Timer();

            t.Interval = 1000/8;
            t.Tick += t_Tick;
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            draw();
        }

        private int frame = 0;
        void draw()
        {
            PixelMap output = new PixelMap((int)document.Width*2, (int)document.Height*2);

            for (int x = 0; x < output.Width; x++)
            {
                for (int y = 0; y < output.Height; y++)
                {
                    output[x, y] = Color.White;
                }
            }

            List<Point> points = new List<Point>();

            var paths = document.Descendants().OfType<SvgPath>();

            for (int i = 0; i < 2; i++)
            {
                foreach (var path in paths)
                {
                    foreach (var line in path.PathData)
                    {
                        var pixels = traceLine((int)line.Start.X * 2, (int)line.Start.Y * 2, (int)line.End.X * 2, (int)line.End.Y * 2);

                        points.AddRange(pixels);
                    }
                }

            }

            foreach (var point in points)
            {
                if (point.X >= 0 && point.X < output.Width && point.Y >= 0 && point.Y < output.Height)
                    output[point.X, point.Y] = Color.Black;
            }

            var bitmap = output.Bitmap;

            pictureBox1.Image = bitmap;

            //bitmap.Save("frames\\"+frame+".png");


            
            frame++;
        }

        private static int randAmount = 0;
        static int rand()
        {
            return r.Next(-randAmount, randAmount + 1);
        }

        static double getAngle(double x, double y)
        {
            return (Math.Atan2(x, y) - Math.Atan2(0, 1)) - Math.PI;

        }

        static Random r = new Random();
        static IEnumerable<Point> traceLine(int x0, int y0, int x1, int y1)
        {
            x0 = x0 + rand();
            x1 = x1 + rand();

            double vel = 5d;
            double X = x1 + rand();
            double Y = y1 + rand();
            double angle = getAngle(X - x0, Y - y0);

            double adjust = 0.2d;
            Point last = new Point((int)X, (int)Y);

            double lastDistance = double.MaxValue;
            for (int i = 0; i < 10000; i++)
            {
                double XVel = vel * Math.Sin(angle);
                double YVel = vel * Math.Cos(angle);

                double XDiff = X - x0;
                double YDiff = Y - y0;

                double distance = Math.Sqrt(XDiff * XDiff + YDiff * YDiff);

                if (distance > lastDistance + (adjust/vel)*2)
                    break;

                lastDistance = distance;


                if (angle < getAngle(XDiff, YDiff))
                    angle += adjust;
                else
                    angle -= adjust;

                angle += (r.NextDouble() - 0.5) * Math.PI / 8;

                adjust += 0.001;

                X += XVel;
                Y += YVel;


                var points = drawLine((int)X, (int)Y, last.X, last.Y);
                last = new Point((int)X, (int)Y);
                foreach (var point in points)
                {
                    yield return point;
                }

            }
        }


        static public List<Point> drawLine(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2; // error value e_xy

            List<Point> points = new List<Point>();

            for (; ; )
            {
                points.Add(new Point(x0, y0));

                if (x0 == x1 && y0 == y1) break;

                e2 = 2 * err;

                // horizontal step?
                if (e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                }

                // vertical step?
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return points;
        }

        static IEnumerable<Point> drawCircle()
        {
            Point offset = new Point(256+32,256);

            for (double i = 0; i < Math.PI*2; i+=(2*Math.PI/16))
            {
                double x = 128 * Math.Cos(i) + offset.X;
                double y = 128 * Math.Sin(i) + offset.Y;

                yield return new Point((int)x,(int)y);
            }

        }
    }
}
