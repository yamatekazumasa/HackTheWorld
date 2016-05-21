using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 実際にプレイヤーを動かしたりして遊ぶシーン
    /// </summary>
    class GameScene : Scene
    {
        // ゲーム画面外の変数の定義
        private List<MenuItem> _menuItem;
        private MenuItem _backButton;
        private MenuItem _resetButton;
        private MenuItem _pauseButton;
        // ゲーム内変数宣言
        private readonly Stage _stage;
        private List<GameObject> _objects;
        private Player _player;
        private List<Block> _blocks;
        private List<IEditable> _editableObjects;
        private List<Enemy> _enemies;
        private List<Item> _items;

        public GameScene()
        {
            _stage = Stage.CreateDemoStage();
            // CodeParser ができた時点でこちらに置き換える。
            // _stage = Stage.CreateDemoStage().Replica;
        }

        public GameScene(Stage stage)
        {
            _stage = stage;
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

            // CodeParser ができていないとeditableObjectsが機能しない。
            // shallow copy だとコンティニュー時に途中からスタートになる。
            var s = _stage.Replica;
            _objects = s.Objects;
            _player = s.Player;
            _blocks = s.Blocks;
            _editableObjects = s.EditableObjects;
            _enemies = s.Enemies;
            _items = s.Items;

            foreach (var o in _editableObjects)
            {
                o.ProcessPtr = 0;
                if (o.Processes != null)
                {
                    o.Compile();
                    o.Execute();
                }
            }

        }

        public override void Update(float dt)
        {
            // ゲーム外処理
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();
            if (_backButton.Clicked || Input.X.Pushed || Input.Back.Pushed) Scene.Pop();
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_resetButton.Clicked) Startup();
            if (_pauseButton.Clicked) Scene.Push(new PauseScene());
            // セーブ・ロード
            if (Input.Control.Pressed)
            {
                if (Input.R.Pushed)
                {
                    var stage = Stage.Load();
                    _objects = stage.Objects;
                    _player = stage.Player;
                    _blocks = stage.Blocks;
                    _editableObjects = stage.EditableObjects;
                    _enemies = stage.Enemies;
                    _items = stage.Items;
                }
                if (Input.S.Pushed)
                {
                    var stage = new Stage {
                        Objects = _objects,
                        Player = _player,
                        Blocks = _blocks,
                        EditableObjects = _editableObjects,
                        Enemies = _enemies,
                        Items = _items
                    };
                    Stage.Save(stage);
                }
            }

            if (_player == null) return;

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
            foreach (var obj in _editableObjects)
            {
                obj.Update(dt);
            }

            // PlayerとBlockが重ならないように位置を調整
            foreach (var block in _blocks)
            {
                _player.Adjust(block);
            }

            // アイテム取得判定
            foreach (var item in _items)
            {
                if (_player.Intersects(item) || item.IsAlive)
                {
                    _player.Y -= CellSize / 4;
                    _player.Height += CellSize / 4;
                    _player.Width  = CellSize;
                    _player.jumpspeed = -CellSize * 13; // h=v^2/2g
                    item.Die();
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
            _objects.ForEach(obj => obj.Draw());

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

    }
}
