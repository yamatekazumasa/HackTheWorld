﻿using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Enemy : GameObject
    {
        public Enemy(int x, int y) : base(x, y)
        {
        }

        public Enemy(int x, int y, int vx, int vy) : base(x, y, vx, vy)
        {
        }

        public Enemy(int x, int y, int vx, int vy, int w, int h) : base(x, y, vx, vy, w, h)
        {
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Yellow, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black   , X, Y, Width, Height);
        }
    }
}
