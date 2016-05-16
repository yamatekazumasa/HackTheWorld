using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;//Image
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public class Player : GameObject
    {
        private Image _img;
        public int speed = CellSize * 3;
        public int jumpspeed = -CellSize * 11; // h=v^2/2g

        public Player()
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            Size = new Vector(CellSize * 7 / 10, CellSize * 9 / 10);
        }

        public override void Update(float dt)
        {
            // キーで動かす部分
            if (Input.Left.Pressed)  X -= speed * dt;
            if (Input.Right.Pressed) X += speed * dt;
//            if (Input.Left.Pressed)  VX = -speed;
//            if (Input.Right.Pressed) VX = speed;
//            if (Input.Left.Pressed == Input.Right.Pressed) VX = 0;
            if (Input.Up.Pushed && OnGround) VY = jumpspeed;
            
            // 自動で動く部分
            VY += Gravity * dt;
            Move(dt);
        }

        /// <summary>
        /// 乗り判定。
        /// 渡されたオブジェクトの矩形領域の上辺に(重ならずに)接触しているか判定する。
        /// </summary>
        /// <param name="obj">渡されたオブジェクト。</param>
        /// <returns>乗っていたらtrue、乗っていなかったらfalseを返す。</returns>
        public virtual bool StandOn(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX &&
                   MaxY > obj.MinY - 1 && MaxY <= obj.MinY;//この行自信ないです
        }

        public virtual bool HitHeadOn(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX &&
                   MinY > obj.MaxY - 1 && MinY <= obj.MaxY;//この行自信ないです
        }

        public override void Draw()
        {
            GraphicsContext.DrawImage(_img, X, Y, Width, Height);
            //GraphicsContext.FillRectangle(Brushes.Aqua, X, Y, Width, Height);
            //GraphicsContext.DrawRectangle(Pens.LightBlue, X, Y, Width, Height);
            if (!IsAlive) GraphicsContext.FillRectangle(Brushes.Gray, X, Y, Width, Height);
        }

    }
}
