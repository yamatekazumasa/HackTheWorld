using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameScene : Scene
    {
        // ゲーム画面外の変数の定義
        private readonly List<MenuItem> _menuItem = new List<MenuItem>();
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"), Image.FromFile(@"image\back1.bmp"));
        private readonly MenuItem _resetButton = new MenuItem(Image.FromFile(@"image\reset.jpg"), Image.FromFile(@"image\reset1.bmp"));
        private readonly MenuItem _pauseButton = new MenuItem(Image.FromFile(@"image\stop.jpg"), Image.FromFile(@"image\stop1.bmp"));
        // ゲーム内変数宣言
        Image _img;
        Player _player;
        List<GameObject> _blocks;
        List<ProcessfulObject> _pblocks;
        List<Enemy> _enemies;
        List<Item> _items;
        private Stage _stage;

        public GameScene(Stage stage)
        {
            _stage = stage;
        }

        public override void Cleanup()
        {
//            _img.Dispose();
//            _stage = null;
//            _player = null;
        }

        public override void Startup()
        {
            // ゲーム画面外初期化
            _backButton.Size = new Vector(50, 50);
            _backButton.Position = new Vector(25, 600);
            _resetButton.Size = new Vector(50, 50);
            _resetButton.Position = new Vector(75, 600);
            _pauseButton.Size = new Vector(50, 50);
            _pauseButton.Position = new Vector(125, 600);
            _menuItem.Add(_backButton); _menuItem.Add(_resetButton); _menuItem.Add(_pauseButton);

            // ゲーム内初期化
            // 変数の初期化
            _img = Image.FromFile(@"image\masato1.jpg");
            _stage = _stage ?? Stage.Load();
            _player = new Player(_img);
            _blocks = new List<GameObject>();
            _pblocks = new List<ProcessfulObject>();
            _enemies = new List<Enemy>();
            _items = new List<Item>();
            _stage = new Stage();
            
            // マップの生成
            for (int iy = 0; iy < CellNumY; iy++)
            {
                for (int ix = 0; ix < CellNumX; ix++)
                {
                    if (Map[iy, ix] == 1)
                    {
                        var block = new Block(CellSize * ix, CellSize * iy);
                        _stage.Objects.Add(block);
                        _blocks.Add(block);
                    }
                    if (Map[iy, ix] == 11)
                    {
                        var pblock = new PBlock(CellSize * ix, CellSize * iy);
                        GetProcess(pblock);
                        _stage.Objects.Add(pblock);
                        _blocks.Add(pblock);
                        _pblocks.Add(pblock);
                    }
                    if (Map[iy, ix] == 2)
                    {
                        var enemy = new Enemy(CellSize * ix, CellSize * iy);
                        _stage.Objects.Add(enemy);
                        _enemies.Add(enemy);
                    }
                    if (Map[iy, ix] == 3)
                    {
                        var item = new Item(CellSize * ix, CellSize * iy);
                        _stage.Objects.Add(item);
                        _items.Add(item);
                    }
                }
            }
        }

        public override void Update(float dt)
        {
            // ゲーム外処理
            if (Input.Sp2.Pushed || Input.Back.Pushed) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();
            // ボタンの処理
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_resetButton.Clicked) Startup();
            if (_pauseButton.Clicked) Scene.Push(new PauseScene());
            // セーブ・ロード
            if (Input.Control.Pressed)
            {
                if (Input.R.Pushed)
                {
                    _stage = Stage.Load();
                }
                if (Input.S.Pushed)
                {
                    Stage.Save(_stage);
                }
            }

            // ゲーム内処理
            // 死亡時処理
            if (!_player.IsAlive)
            {
                Scene.Push(new ContinueScene());
            }

            // オブジェクトの移動
            _player.OnGround = false;
            foreach (var block in _blocks)
            {
                if (_player.StandOn(block))
                {
                    _player.OnGround = true;
                    if (_player.VY > 0) _player.VY = 0; //速度正なら、という条件は必要です
                    _player.Y += block.VY*dt;
                    _player.X += block.VX*dt;
                }
                if (_player.HitHeadOn(block))
                {
                    _player.VY = 0;
                }
            }

            _player.Update(dt);
            foreach (var pblock in _pblocks)
            {
                pblock.Update(dt);
            }

            // PlayerとBlockが重ならないように位置を調整
            foreach (var block in _blocks)
            {
                _player.Adjust(block);
            }

            // アイテム取得判定
            for (int i = _items.Count; --i >= 0;)
            {
                if (_player.Intersects(_items[i]))
                {
                    _player.Y -= CellSize / 4;
                    _player.Height += CellSize / 4;
                    _player.Width  = CellSize;
                    _stage.Objects.Remove(_items[i]);
                    _items.RemoveAt(i);
                }
            }

            // 死亡判定
            foreach (var enemy in _enemies)
            {
                if (_player.Intersects(enemy)) _player.Die();
            }

            // 画面のクリア
            ScreenClear();

            // 描画
            foreach (var obj in _stage.Objects)
            {
                obj.Draw();
            }
            _player.Draw();

            // ボタンの描画
            foreach (var menuitem in _menuItem)
            {
                menuitem.Draw();
            }
        }

        private void ScreenClear()
        {
            GraphicsContext.Clear(Color.White);
            for (int ix = 0; ix < ScreenWidth; ix += CellSize)
            {
                GraphicsContext.DrawLine(Pens.LightGray, ix, 0, ix, ScreenHeight);
            }
            for (int iy = 0; iy < ScreenHeight; iy += CellSize)
            {
                GraphicsContext.DrawLine(Pens.LightGray, 0, iy, ScreenWidth, iy);
            }
        }

        private void GetProcess(ProcessfulObject pobj)
        {
            pobj.SetProcesses(new Process[] {
                            new Process((obj, dt) => { ; } , 3.0f),

                            new Process((obj, dt) => { obj.VY = -CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.VY = 0; } , 2.0f),

                            new Process((obj, dt) => { obj.VY = +CellSize; }, 1.0f),
                            new Process((obj, dt) => { ; } , 2.0f),
                            new Process((obj, dt) => { obj.VY = 0; } , 0.01f),

                            new Process((obj, dt) => { obj.VX = -CellSize; }, 1.0f),
                            new Process((obj, dt) => { obj.VX = 0; } , 2.0f),

                            new Process((obj, dt) => { ; } , 2.0f),
                            new Process((obj, dt) => { obj.Y -= dt*CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.Y += dt*CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.X += dt*CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.X -= dt*CellSize; }, 3.0f),
                        });
        }
    }
}
