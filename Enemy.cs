using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Enemy : GameObject
    {
        public Enemy(float x, float y) : base(x, y)
        {
        }

        public Enemy(float x, float y, float vx, float vy) : base(x, y, vx, vy)
        {
        }

        public Enemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h)
        {
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.HotPink, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Magenta, X, Y, Width, Height);
        }
    }
}
