﻿using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class PBlock : ProcessfulObject
    {
        public bool isWorking = false;
        public PBlock(int x, int y) : base(x, y)
        {
        }

        public override void Update(float dt)
        {
            //if (isWorking)
            //{
                base.Update(dt);
                Move(dt);
            //}
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Gold, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Orange, X, Y, Width, Height);
        }
    }
}
