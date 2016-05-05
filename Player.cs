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

        public Player(Image img)
        {
            this._img = img;

            this.Size = new Vector(CellSize, CellSize);

        }

        public override void Update(float dt)
        {
            int speed = 100;
            if (Input.Left.Pressed)  X -= speed * dt;
            if (Input.Right.Pressed) X += speed * dt;
            if (Input.Up.Pushed)     VY = -1000;
            if (Input.Down.Pressed)  Y += speed * dt;

            VY += 2000 * dt;

            Move(dt);
           
        }

        public override void Draw()
        {
            GraphicsContext.DrawImage(_img, X, Y, Width, Height);
        }

    }

}
