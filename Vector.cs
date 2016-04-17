using System;
using System.Drawing;

namespace HackTheWorld
{

    public struct Vector
    {
        public double X { set; get; }
        public double Y { set; get; }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector Rotate(double deg)
        {
            double theta = deg * Math.PI / 180;
            X = Math.Cos(theta) * X - Math.Sin(theta) * Y;
            Y = Math.Sin(theta) * X + Math.Cos(theta) * Y;
            return this;
        }

        public Vector Extend(double d)
        {
            return (d + Length()) / d * this;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator *(double b, Vector a)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator /(Vector a, double b)
        {
            return new Vector(a.X / b, a.Y / b);
        }

        public static implicit operator Size(Vector v)
        {
            return new Size((int)v.X, (int)v.Y);
        }

        public static implicit operator Point(Vector v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

    }

}
