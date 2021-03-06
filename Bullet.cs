﻿using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 弾。決して Bullet が Bullet を生成してはならない。
    /// </summary>
    public class Bullet : GameObject
    {
        public Bullet(float x, float y) : base(x, y) { }

        public Bullet(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Bullet(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector(10, 10);
        }

        public override void Draw()
        {
            if (!IsAlive) return;
            GraphicsContext.FillPie(Brushes.Red, X, Y, Width, Height, 0, 360);
        }
    }
}
