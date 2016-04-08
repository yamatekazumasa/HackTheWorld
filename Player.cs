using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;//Image
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Player
    {
        public Vector position;
        //public Vector size;
        float speed = 5;    // 速度

        public Player(Vector position)
        {
            this.position = position;
            //this.size = new Vector(30, 30);
        }


        public void move()//押されている間動かす
        {
            if (Input.Left.Pressed)     position += new Vector(-speed, 0);
            if (Input.Right.Pressed)    position += new Vector(+speed, 0);
            if (Input.Up.Pressed)       position += new Vector(0, -speed);
            if (Input.Down.Pressed)     position += new Vector(0, +speed);
        }
        public void draw(Image img)
        {
            GraphicsContext.DrawImage(img, (float)position.X, (float)position.Y);//float型に変換
        }
    }
}
