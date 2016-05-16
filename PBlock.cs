using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public class PBlock : EditableObject
    {
        public bool IsWorking = false;
        public PBlock(int x, int y) : base(x, y) { }

        public override void Draw()
        {
            base.Draw();
            GraphicsContext.FillRectangle(Brushes.Gold, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black, X, Y, Width, Height);
        }
    }
}
