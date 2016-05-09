using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class PBlock : ProcessfulObject
    {
        public bool isWorking = false;
        public PBlock(int x, int y) : base()
        {
            X = x;
            Y = y;
            VX = 0;
            VY = 0;
            Width = CellSize;
            Height = CellSize;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            Move(dt);
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Gold, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black, X, Y, Width, Height);
        }
    }
}
