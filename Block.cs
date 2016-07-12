using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// ブロック。
    /// サイズは CellSize で固定。
    /// </summary>
    public class Block : GameObject, IDrawable
    {
        public Block(float x, float y) : base(x, y) { }

        public Block(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Block(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public Image Image { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Image = Image.FromFile(@"image\masato1.jpg");
        }

        public override void Draw()
        {
            ((IDrawable) this).Draw();
//            GraphicsContext.FillRectangle(Brushes.Brown, X, Y, Width, Height);
//            GraphicsContext.DrawRectangle(Pens.Black   , X, Y, Width, Height);
        }

    }
}
