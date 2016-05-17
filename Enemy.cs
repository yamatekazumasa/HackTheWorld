using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Enemy : GameObject
    {
        public Enemy(float x, float y) : base(x, y)
        {
            ObjectType = ObjectType.Enemy;
        }

        public Enemy(float x, float y, float vx, float vy) : base(x, y, vx, vy)
        {
            ObjectType = ObjectType.Enemy;
        }

        public Enemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h)
        {
            ObjectType = ObjectType.Enemy;
        }

        public override void Draw()
        {
            //GraphicsContext.FillRectangle(Brushes.HotPink, X, Y, Width, Height);
            GraphicsContext.FillPie(Brushes.HotPink, X, Y, Width, Height, 0, 360);
            GraphicsContext.DrawRectangle(Pens.Magenta, X, Y, Width, Height);
        }
    }
}
