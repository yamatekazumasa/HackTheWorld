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
            this.Size = new Vector(Cell, Cell);
        }

        public override void Update()
        {
            int speed = 2;
            if (Input.Left.Pressed)  X -= speed;
            if (Input.Right.Pressed) X += speed;
            if (Input.Up.Pushed)     VY = -800;
            if (Input.Down.Pressed)  Y += speed;

            VY += 20;

            Move();

        }

        public override void Draw()
        {
            GraphicsContext.DrawImage(_img, X, Y, Width, Height);
        }

    }
}
