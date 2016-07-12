using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 敵用のクラス。
    /// </summary>
    public class Enemy : GameObject, IDrawable
    {
        public Enemy(float x, float y) : base(x, y) { }

        public Enemy(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Enemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public Image Image { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Image = Image.FromFile(@"image\needle.png");
        }

        public override void Draw()
        {
            if (!IsAlive) return;
            ((IDrawable)this).Draw();
        }

    }
}
