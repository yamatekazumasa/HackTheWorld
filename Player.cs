using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;//Image
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// プレイヤー
    /// </summary>
    public class Player : GameObject
    {
        private readonly Image _img;
        public int Speed = CellSize * 3;
        public int Jumpspeed = -CellSize * 11; // h=v^2/2g

        public Player() : base(0,0)
        {
            ObjectType = ObjectType.Player;
            _img = Image.FromFile(@"image\masato1.jpg");
            Size = new Vector(CellSize * 7 / 10, CellSize * 9 / 10);
        }

        public override void Update(float dt)
        {
            // キーで動かす部分
            if (Input.Left.Pressed)  X -= Speed * dt;
            if (Input.Right.Pressed) X += Speed * dt;
            if (Input.Up.Pushed && OnGround) VY = Jumpspeed;
            
            // 自動で動く部分
            if (!OnGround) VY += Gravity * dt;
           // if (!OnGround) VX = 0;
            Move(dt);
        }

        /// <summary>
        /// 乗り判定。
        /// 渡されたオブジェクトの矩形領域の上辺に接触しているか判定する。
        /// </summary>
        /// <param name="obj">渡されたオブジェクト。</param>
        /// <returns>乗っていたらtrue、乗っていなかったらfalseを返す。</returns>
        public virtual bool StandOn(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX && (int)MaxY == (int)obj.MinY;
        }

        /// <summary>
        /// 渡されたオブジェクトの矩形領域の下辺に接触しているか判定する。
        /// </summary>
        /// <param name="obj">渡されたオブジェクト。</param>
        /// <returns>頭が当たっていたらtrue、当たっていなかったらfalseを返す。</returns>
        public virtual bool HitHeadOn(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX && (int)MinY == (int)obj.MaxY;
        }

        public override void Draw()
        {
            if (Scene.Current is GameScene) GraphicsContext.DrawImage(_img, X, Y, Width, Height);
            //GraphicsContext.FillRectangle(Brushes.Aqua, X, Y, Width, Height);
            //GraphicsContext.DrawRectangle(Pens.LightBlue, X, Y, Width, Height);
            if (!IsAlive) GraphicsContext.FillRectangle(Brushes.Gray, X, Y, Width, Height);
        }

    }
}
