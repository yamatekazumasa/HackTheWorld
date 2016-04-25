using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;//Image
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Player : GameObject
    {
        private Image _img;
        public bool onGround = false;

        public Player(Image img)
        {
            this._img = img;
            this.Size = new Vector(Cell, Cell);
        }

        public override void Update(float dt)
        {
            // キーで動かす部分
            int speed = 100;
            if (Input.Left.Pressed)  X -= speed * dt;
            if (Input.Right.Pressed) X += speed * dt;
            if (Input.Up.Pushed && onGround)
            {
                onGround = false;
                VY = -1000;
            }
            //if (Input.Down.Pressed)  Y += speed * dt;

            // 自動で動く部分
            int gravity = 2000;
            VY += gravity * dt;
            Move(dt);

        }

        public bool StandOn(GameObject obj)
        {
            return true;
        }

        public bool HitHeadOn(GameObject obj)
        {
            return true;
        }

        public override void Draw()
        {
            GraphicsContext.DrawImage(_img, X, Y, Width, Height);
        }

    }
}
