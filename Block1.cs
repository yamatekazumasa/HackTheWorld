using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Block1 : Block
    {
        public Block1(int x, int y) : base(x, y)
        {
            //work = true;
        }

        public override void Update(float dt)
        {
            if (isWorking)
            {
                Move(dt);
            }
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Chocolate, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black, X, Y, Width, Height);
        }
    }
}
