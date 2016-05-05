using System;
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
            // playerの初期化
            _img = Image.FromFile(@"image\masato1.jpg");
            _player = new Player(_img);
//<<<<<<< HEAD
//            _player.Initialize(ObjectType.Player);
//            // ブロックの初期化
//            _blocks = new List<GameObject>();
//            for (int i=0;i<5; i+=2){
//                GameObject b = new GameObject(Cell*i, Cell*10);
//                b.Initialize(ObjectType.Block);
//                _blocks.Add(b);

//=======

            // ブロックの初期化
            _blocks = new List<GameObject>();
            for (int iy = 0; iy < CellNumY; iy++)
            {
                for (int ix = 0; ix < CellNumX; ix++)
                {
                    if (Map[iy, ix] == 1)
                    {
                        _blocks.Add(new GameObject(CellSize * ix, CellSize * iy));
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
            if (_player.IsAlive)
            {
                _player.Update(dt);
            }
            foreach (var block in _blocks)
            {
                if (_player.Intersects(block))
                {
                    _player.VY = 0;
                }
                _player.Adjust(block);
            }
//<<<<<<< HEAD
            
//            GraphicsContext.Clear(Color.White);

//            //GraphicsContext.DrawImage(_img, 0, 0);

//            //ここに作成
//            // 描画のみ
//            for (int ix=0;ix<ScreenWidth;ix+=Cell)
//            {
//                GraphicsContext.DrawLine(Pens.Gray, ix, 0, ix, ScreenHeight);
//            }
//            for (int iy = 0; iy < ScreenHeight; iy += Cell)
//            {
//                GraphicsContext.DrawLine(Pens.Gray, 0, iy, ScreenWidth, iy);
//            }


//=======
            //死亡
            if (_player.Y > ScreenHeight)
            {
                _player.Die();
                Scene.Push(new ContinueScene());
            }

            // 画面のクリア
            ScreenClear();

            // 描画

            _player.Draw();
            foreach (var block in _blocks)
            {
                block.Draw();

            }
            //ボタンの描写
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
