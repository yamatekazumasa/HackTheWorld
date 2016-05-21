using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// ブロック。
    /// サイズは CellSize で固定。
    /// </summary>
    public class Block : GameObject
    {
        public Block(float x, float y) : base(x, y) { }

        public Block(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Block(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            ObjectType = ObjectType.Block;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Brown, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black   , X, Y, Width, Height);
        }
    }
}
