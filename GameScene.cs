using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameScene : Scene
    {
        // ゲーム画面外の変数の定義
        private List<MenuItem> _menuItem;
        private MenuItem _backButton;
        private MenuItem _resetButton;
        private MenuItem _pauseButton;
        // ゲーム内変数宣言
        private Player _player;
        private List<GameObject> _blocks;
        private List<EditableObject> _pblocks;
        private List<Enemy> _enemies;
        private List<Item> _items;
        private Stage _stage;

        public GameScene(Stage stage)
        {
            _stage = new Stage {
                Rows = stage.Rows,
                Cols = stage.Cols,
                Objects = new List<GameObject>()
            };
            foreach (var obj in stage.Objects)
            {
                _stage.Objects.Add(obj);
            }
        }

        public override void Cleanup()
        {
//            _stage = null;
//            _player = null;
        }

        public override void Startup()
        {
            // ゲーム画面外初期化
            _backButton = new MenuItem(Image.FromFile(@"image\back.png"), Image.FromFile(@"image\back1.bmp")) {
                Size = new Vector(50, 50),
                Position = new Vector(25, 600)
            };
            _resetButton = new MenuItem(Image.FromFile(@"image\reset.jpg"), Image.FromFile(@"image\reset1.bmp")) {
                Size = new Vector(50, 50),
                Position = new Vector(75, 600)
            };
            _pauseButton = new MenuItem(Image.FromFile(@"image\stop.jpg"), Image.FromFile(@"image\stop1.bmp")) {
                Size = new Vector(50, 50),
                Position = new Vector(125, 600)
            };
            _menuItem = new List<MenuItem> {_backButton, _resetButton, _pauseButton};

            // ゲーム内初期化
            // 変数の初期化
            _stage = _stage ?? new Stage();
            _player = new Player();
            _blocks = new List<GameObject>();
            _pblocks = new List<EditableObject>();
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
//                        _stage.Objects.Add(pblock);
//                        _blocks.Add(pblock);
//                        _pblocks.Add(pblock);
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
            if (Input.X.Pushed || Input.Back.Pushed) Scene.Pop();
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
                    _player.jumpspeed = -CellSize * 13; // h=v^2/2g
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
            DebugWrite();

            // 描画
            _stage.Objects.ForEach(obj => obj.Draw());
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
            GraphicsContext.DrawImage(Image.FromFile(@"image\backscreen.bmp"),0,0);
            for (int ix = 0; ix < ScreenWidth; ix += CellSize)
            {
                GraphicsContext.DrawLine(Pens.LightGray, ix, 0, ix, ScreenHeight);
            }
            for (int iy = 0; iy < ScreenHeight; iy += CellSize)
            {
                GraphicsContext.DrawLine(Pens.LightGray, 0, iy, ScreenWidth, iy);
            }
        }

        private void DebugWrite()
        {
            string PX = " X: " + ((int)(_player.X * 1000 / CellSize)).ToString("D6") + "#";
            string PY = " Y: " + ((int)(_player.Y * 1000 / CellSize)).ToString("D6") + "#";
            string PVX = "VX: " + ((int)(_player.VX * 1000 / CellSize)).ToString("D6") + "#";
            string PVY = "VY: " + ((int)(_player.VY * 1000 / CellSize)).ToString("D6") + "#";
            Font font = new Font("Courier New", 12);
            GraphicsContext.DrawString(PX, font, Brushes.Black, ScreenWidth - 180, 100);
            GraphicsContext.DrawString(PY, font, Brushes.Black, ScreenWidth - 180, 120);
            GraphicsContext.DrawString(PVX, font, Brushes.Black, ScreenWidth - 180, 140);
            GraphicsContext.DrawString(PVY, font, Brushes.Black, ScreenWidth - 180, 160);
        }

        private void GetProcess(EditableObject pobj)
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
