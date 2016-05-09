using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Block : GameObject
    {
        public Block(int x, int y) : base(x, y)
        {
            Type = ObjectType.Block;
        }

        public Block(int x, int y, int vx, int vy) : base(x, y, vx, vy)
        {
            Type = ObjectType.Block;
        }

        public Block(int x, int y, int vx, int vy, int w, int h) : base(x, y, vx, vy, w, h)
        {
            Type = ObjectType.Block;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Brown, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black   , X, Y, Width, Height);
        }
    }
}
