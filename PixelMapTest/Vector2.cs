using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelMapTest
{
    struct Vector2
    {
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 FromTheta(double theta, double magnitude)
        {
            return new Vector2(magnitude * Math.Sin(theta), magnitude * Math.Cos(theta));
        }
        public double X;
        public double Y;

        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }

        public double Theta
        {
            get { return Math.Atan2(0, 1) - Math.Atan2(Y, X) + Math.PI; }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }
}
