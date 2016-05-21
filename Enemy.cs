using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 敵用のクラス。
    /// </summary>
    public class Enemy : GameObject
    {
        public Enemy(float x, float y) : base(x, y) { }

        public Enemy(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Enemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
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
