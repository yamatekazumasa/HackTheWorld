using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public interface IDrawable
    {
        float X { get; set; }
        float Y { get; set; }
        float W { get; set; }
        float H { get; set; }
        Image Image { get; set; }
    }

    public static partial class Extensions
    {
        public static void Draw(this IDrawable self)
        {
            GraphicsContext.DrawImage(self.Image, self.X, self.Y, self.W, self.H);
        }
    }
}


