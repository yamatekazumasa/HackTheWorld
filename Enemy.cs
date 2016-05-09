using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Enemy : GameObject
    {
        public Enemy(int x, int y) : base(x, y)
        {
        }

        public Enemy(int x, int y, int vx, int vy) : base(x, y, vx, vy)
        {
        }

        public Enemy(int x, int y, int vx, int vy, int w, int h) : base(x, y, vx, vy, w, h)
        {
        }

        public override void Draw()
        {
            //GraphicsContext.FillRectangle(Brushes.HotPink, X, Y, Width, Height);
            GraphicsContext.FillPie(Brushes.HotPink, X, Y, Width, Height, 0, 360);
            GraphicsContext.DrawRectangle(Pens.Magenta, X, Y, Width, Height);
        }
    }
}
