using System;
using System.Drawing;

namespace HackTheWorld
{
    /// <summary>
    /// 座標、サイズなど 2 次元のデータを double 型で保存するクラス。
    /// </summary>
    public struct Vector
    {
        /// <summary>
        /// 第一成分。
        /// </summary>
        public double X { set; get; }
        /// <summary>
        /// 第二成分。
        /// </summary>
        public double Y { set; get; }

        /// <summary>
        /// ベクトルを生成する。
        /// </summary>
        /// <param name="x">第一成分。</param>
        /// <param name="y">第二成分。</param>
        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// ベクトルを回転する。
        /// </summary>
        /// <param name="deg">回転角（degree）</param>
        public Vector Rotate(double deg)
        {
            double theta = deg * Math.PI / 180;
            X = Math.Cos(theta) * X - Math.Sin(theta) * Y;
            Y = Math.Sin(theta) * X + Math.Cos(theta) * Y;
            return this;
        }

        /// <summary>
        /// ベクトルを伸長する。
        /// </summary>
        /// <param name="d">伸ばす長さ。</param>
        public Vector Extend(double d)
        {
            return (d + Length) / d * this;
        }

        /// <summary>
        /// ベクトルの長さ。
        /// </summary>
        public double Length => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// ベクトルの加算。
        /// </summary>
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// ベクトルの減算。
        /// </summary>
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// ベクトルのスカラー倍。
        /// </summary>
        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        /// <summary>
        /// ベクトルのスカラー倍。
        /// </summary>
        public static Vector operator *(double b, Vector a)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        /// <summary>
        /// ベクトルをスカラーで割る。
        /// </summary>
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

        /// <summary>
        /// ベクトルを暗黙的に Size に変換する。
        /// </summary>
        public static implicit operator Size(Vector v)
        {
            return new Size((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// ベクトルを暗黙的に SizeF に変換する。
        /// </summary>
        public static implicit operator SizeF(Vector v)
        {
            return new SizeF((float)v.X, (float)v.Y);
        }

        /// <summary>
        /// ベクトルを暗黙的に Point に変換する。
        /// </summary>
        public static implicit operator Point(Vector v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// ベクトルを暗黙的に PointF に変換する。
        /// </summary>
        public static implicit operator PointF(Vector v)
        {
            return new PointF((float)v.X, (float)v.Y);
        }

    }

}
