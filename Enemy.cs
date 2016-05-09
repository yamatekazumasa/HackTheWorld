using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Enemy : GameObject
    {
        public Enemy(float x, float y) : base(x, y)
        {
            Type = ObjectType.Enemy;
        }

        public Enemy(float x, float y, float vx, float vy) : base(x, y, vx, vy)
        {
            Type = ObjectType.Enemy;
        }

        public Enemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h)
        {
            Type = ObjectType.Enemy;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.HotPink, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Magenta, X, Y, Width, Height);
        }
    }
}
