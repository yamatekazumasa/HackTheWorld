using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Item : GameObject
    {
        public Item(int x, int y) : base(x, y)
        {
        }

        public Item(int x, int y, int vx, int vy) : base(x, y, vx, vy)
        {
        }

        public Item(int x, int y, int vx, int vy, int w, int h) : base(x, y, vx, vy, w, h)
        {
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Pink, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Magenta   , X, Y, Width, Height);
        }
    }
}
