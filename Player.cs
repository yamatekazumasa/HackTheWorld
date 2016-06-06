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
    public class Player : GameObject, IAnimatable
    {
        public int Speed = CellSize * 3;
        public int Jumpspeed = -CellSize * 11; // h=v^2/2g
        public Animation Anim { get; set; }

        public Player()
        {
            ObjectType = ObjectType.Player;
            Image img1 = Image.FromFile(@"image\masato1.jpg");
            Image img2 = Image.FromFile(@"image\masato2.jpg");
            Image img3 = Image.FromFile(@"image\masato3.jpg");
            Size = new Vector(CellSize * 7 / 10, CellSize * 9 / 10);
            this.SetAnimation(new[] {img1, img2, img3}, new[] {0.5f, 1.0f, 1.5f});
            Anim.Start();
        }

        public override void Update(float dt)
        {
            // キーで動かす部分
            if (Input.Left.Pressed)  VX = -Speed;
            if (Input.Right.Pressed) VX = Speed;
            if (Input.Left.Pressed == Input.Right.Pressed) VX = 0;
            if (Input.Up.Pushed && OnGround) VY = Jumpspeed;
            
            // 自動で動く部分
            if(!OnGround) VY += Gravity * dt;
            Move(dt);

            // アニメーションを dt 進める
            Anim.Advance(dt);
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
            if (!(Scene.Current is GameScene || Scene.Current is EditMapScene)) return;
            //GraphicsContext.FillRectangle(Brushes.Aqua, X, Y, Width, Height);
            //GraphicsContext.DrawRectangle(Pens.LightBlue, X, Y, Width, Height);
            if (IsAlive) Anim.Draw(VX > 0);
            else         GraphicsContext.FillRectangle(Brushes.Gray, X, Y, Width, Height);
        }

    }
}
