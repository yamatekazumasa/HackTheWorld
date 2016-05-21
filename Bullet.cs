using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public class Bullet : GameObject
    {
        public Bullet(float x, float y) : base(x, y) { }

        public Bullet(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Bullet(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            ObjectType = Constants.ObjectType.Enemy;
            Size = new Vector(10, 10);
        }

        public override void Draw()
        {
            if (!IsAlive) return;
            GraphicsContext.FillPie(Brushes.Red, X, Y, Width, Height, 0, 360);
        }
    }
}
