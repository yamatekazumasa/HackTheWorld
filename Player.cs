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
        public int speed = CellSize * 3;
        public int jumpspeed = -CellSize * 11; // h=v^2/2g
        public int gravity = CellSize * 25;

        public Player(Image img) : base()
        {
            this._img = img;
            this.Size = new Vector(CellSize * 7 / 10, CellSize * 9 / 10);
        }

        public override void Update(float dt)
        {
            // キーで動かす部分
            if (Input.Left.Pressed) VX = -speed;
            if (Input.Right.Pressed) VX = speed;
            if (Input.Up.Pushed && onGround)
            {
                //onGround = false; // 現時点では必要ない
                VY = jumpspeed;
            }

            // 自動で動く部分
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
                   MaxY > obj.MinY - 1 && MaxY <= obj.MinY;//この行自信ないです
        }

        public virtual bool HitHeadOn(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX &&
                   MinY > obj.MaxY - 1 && MinY <= obj.MaxY;//この行いつか書き換えたいです
        }

        public override void Draw()
        {
            GraphicsContext.DrawImage(_img, X, Y, Width, Height);
            GraphicsContext.FillRectangle(Brushes.Aqua, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.LightBlue, X, Y, Width, Height);
            GraphicsContext.DrawLine(Pens.LightBlue, ScreenWidth/2, ScreenHeight/2, X, Y);
            if (!IsAlive) GraphicsContext.FillRectangle(Brushes.Gray, X, Y, Width, Height);
        }

    }
}
