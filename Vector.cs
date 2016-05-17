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

        public static bool operator ==(Vector a, Vector b)
        {
            return Equals(a.X, b.X) && Equals(a.Y, b.Y);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !Equals(a.X, b.X) || !Equals(a.Y, b.Y);
        }

        public bool Equals(Vector other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector && Equals((Vector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static implicit operator Size(Vector v)
        {
            return new Size((int)v.X, (int)v.Y);
        }

        public static implicit operator Point(Vector v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        public static implicit operator PointF(Vector v)
        {
            return new PointF((float)v.X, (float)v.Y);
        }

        public static implicit operator SizeF(Vector v)
        {
            return new SizeF((float)v.X, (float)v.Y);
        }

    }

}
