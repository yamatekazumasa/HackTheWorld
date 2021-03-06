﻿using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// プレイヤー
    /// </summary>
    public sealed class Player : GameObject, IAnimatable
    {
        public int Speed = CellSize * 3;
        public int Jumpspeed = -CellSize * 11; // h=v^2/2g
        public Animation Anim { get; set; }

        public Player()
        {
            Initialize();
        }

        public Player(float x, float y)
        {
            X = x;
            Y = y;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            Image img1 = Image.FromFile(@"image\masato1.jpg");
            Image img2 = Image.FromFile(@"image\masato2.jpg");
            Image img3 = Image.FromFile(@"image\masato3.jpg");
            Size = new Vector(CellSize * 7 / 10, CellSize * 9 / 10);
            this.SetAnimation(new[] { img1, img2, img3 }, new[] { 0.5f, 1.0f, 1.5f });
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

            if (!InWindow()) Die();

            // アニメーションを dt 進める
            Anim.Advance(dt);
        }

        public override void Draw()
        {
            if (IsAlive) Anim.Draw(VX > 0);
            else         GraphicsContext.FillRectangle(Brushes.Gray, X, Y, Width, Height);
        }

    }
}
