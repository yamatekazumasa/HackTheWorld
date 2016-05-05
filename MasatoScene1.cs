﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MasatoScene1 : Scene
    {
        // ゲーム画面外の変数の定義
        private readonly List<MenuItem> _menuItem = new List<MenuItem>();
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"), Image.FromFile(@"image\back1.bmp"));
        private readonly MenuItem _resetButton = new MenuItem(Image.FromFile(@"image\reset.jpg"), Image.FromFile(@"image\reset1.bmp"));
        private readonly MenuItem _pauseButton = new MenuItem(Image.FromFile(@"image\stop.jpg"),Image.FromFile(@"image\stop1.bmp"));
        // ゲーム内変数宣言
        Image _img;
        Player _player;
        List<GameObject> _blocks;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            // ゲーム画面外初期化
            _backButton.Size = new Vector(50, 50);
            _backButton.Position = new Vector(25, 600);
            _resetButton.Size = new Vector(50,50);
            _resetButton.Position = new Vector(75,600);
            _pauseButton.Size = new Vector(50,50);
            _pauseButton.Position = new Vector(125, 600);
            _menuItem.Add(_backButton);_menuItem.Add(_resetButton);_menuItem.Add(_pauseButton);

            // ゲーム内初期化
            // 変数の初期化
            _img = Image.FromFile(@"image\masato1.jpg");
            _player = new Player(_img);

            // ブロックの初期化
            _player.Initialize();
            _blocks = new List<GameObject>();
            // マップの生成
            for (int iy = 0; iy < CellNumY; iy++)
            {
                for (int ix = 0; ix < CellNumX; ix++)
                {
                    if (Map[iy, ix] == 1)
                    {
                        _blocks.Add(new Block(CellSize * ix, CellSize * iy));
                    }
                }

            }
        }

        public override void Update(float dt)
        {
            // ゲーム外処理
            if (Input.Sp2.Pushed) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();
            //ボタンの処理
            foreach (var button in _menuItem)
            {
                button.IsSelected = false;
                if (button.Contains(Input.Mouse.Position)) button.IsSelected = true;
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_resetButton.Clicked) Startup();
            if (_pauseButton.Clicked) Scene.Push(new PauseScene());

            // ゲーム内処理
            // 死亡時処理
            if (!_player.IsAlive)
            {
                System.Threading.Thread.Sleep(1000);
                Scene.Push(new ContinueScene());
            }

            // 移動する前に行う計算
            _player.onGround = false;
            foreach (var block in _blocks)
            {
                if (_player.StandOn(block))
                {
                    _player.onGround = true;
                    if (_player.VY > block.VY) _player.VY = block.VY;
                }
                if (_player.HitHeadOn(block) && _player.VY < 0)
                {
                    _player.VY = 0;
                }
            }

            // 移動
            _player.Update(dt);

            // 調整
            foreach (var block in _blocks)
            {
                _player.Adjust(block);
            }
            //死亡
            if (_player.Y > ScreenHeight)
            {
                _player.Die();
                Scene.Push(new ContinueScene());

            // 死亡判定
            if (_player.X > CellSize * 15)
            {
                _player.Die();
            }

            // 画面のクリア
            ScreenClear();

            // 描画

            _player.Draw();
            foreach (var block in _blocks)
            {
                block.Draw();

            }

            // ゲーム画面外の描画
            // ボタンの描画
            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }

        private void ScreenClear()
        {
            GraphicsContext.Clear(Color.White);
        }
    }
}
