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
            this.Size = new Vector(CellSize * 7 / 10, CellSize * 9 / 10);
        }

        public override void Update(float dt)
        {
            // キーで動かす部分
            int speed = CellSize * 3;
            if (Input.Left.Pressed)  X -= speed * dt;
            if (Input.Right.Pressed) X += speed * dt;
            //if (Input.Down.Pressed)  Y += speed * dt;

            // キーで操作する部分
            if (Input.Up.Pushed && onGround)
            {
                onGround = false;
                VY = -CellSize * 11; // h=v^2/2g
            }

            // 自動で動く部分
            int gravity = CellSize * 25;
            VY += gravity * dt;
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
                   MaxY == obj.MinY;
        }

        public virtual bool HitHeadOn(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX &&
                   MinY == obj.MaxY;
        }

        public override void Draw()
        {
            GraphicsContext.DrawImage(_img, X, Y, Width, Height);
            //GraphicsContext.FillRectangle(Brushes.Aqua, X, Y, Width, Height);
            //GraphicsContext.DrawRectangle(Pens.LightBlue, X, Y, Width, Height);
        }

    }
}
