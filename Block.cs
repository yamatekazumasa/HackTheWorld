using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Block : GameObject
    {
        //public bool work = false;
        //public bool isWorking = false;

        public Block(float x, float y) : base(x, y)
        {
            Type = ObjectType.Block;
        }

        public Block(float x, float y, float vx, float vy) : base(x, y, vx, vy)
        {
            Type = ObjectType.Block;
        }

        public Block(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h)
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
