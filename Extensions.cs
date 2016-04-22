using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    public static class Extensions
    {
        public static Vector ToVector(this Point p)
        {
            return new Vector(p.X, p.Y);
        }

        public static Vector ToVector(this Size s)
        {
            return new Vector(s.Width, s.Height);
        }
    }
}
