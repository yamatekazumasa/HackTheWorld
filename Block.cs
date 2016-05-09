using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Block : GameObject
    {
        public Block(float x, float y) : base(x, y)
        {
            ObjectType = ObjectType.Block;
        }

        public Block(float x, float y, float vx, float vy) : base(x, y, vx, vy)
        {
            ObjectType = ObjectType.Block;
        }

        public Block(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h)
        {
            ObjectType = ObjectType.Block;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Brown, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black   , X, Y, Width, Height);
        }
    }
}
