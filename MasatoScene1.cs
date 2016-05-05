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
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private readonly MenuItem _resetButton = new MenuItem(Image.FromFile(@"image\reset.jpg"));

        // ゲーム内変数宣言
        Image _img;
        Player _player;
        List<GameObject> _blocks;
        List<PBlock> _pblocks;
        List<Enemy> _enemies;
        List<Item> _items;
        bool _tmp_head;//仮

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            // ゲーム画面外初期化
            _backButton.Size = new Vector(50, 50);
            _backButton.Position = new Vector(25, 600);
            _resetButton.Size = new Vector(100,50);
            _resetButton.Position = new Vector(100,600);
            
            // ゲーム内初期化
            // playerの初期化
            _img = Image.FromFile(@"image\masato1.jpg");
            _player = new Player(_img);

            // ブロックの初期化
            _blocks = new List<GameObject>();
            _pblocks = new List<PBlock>();
            _enemies = new List<Enemy>();
            _items = new List<Item>();

            for (int iy = 0; iy < CellNumY; iy++)
            {
                for (int ix = 0; ix < CellNumX; ix++)
                {
                    if (Map[iy, ix] == 1)
                    {
                        _blocks.Add(new Block(CellSize * ix, CellSize * iy));
                    }
                    if (Map[iy, ix] == 2)
                    {
                        var pblock = new PBlock(CellSize * ix, CellSize * iy);
                        GetProcess(pblock);
                        _blocks.Add(pblock);
                        _pblocks.Add(pblock);
                    }
                    if (Map[iy, ix] == 3)
                    {
                        _enemies.Add(new Enemy(CellSize * ix, CellSize * iy));
                    }
                    if (Map[iy, ix] == 4)
                    {
                        _items.Add(new Item(CellSize * ix, CellSize * iy));
                    }
                }
            }

        }

        public override void Update(float dt)
        {
            // ゲーム外処理
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_resetButton.Clicked) Startup();


            // ゲーム内処理
            // 死亡時処理
            if (!_player.IsAlive)
            {
                System.Threading.Thread.Sleep(1000);
                //Scene.Push(new ContinueScene());
                Scene.Pop();
            }

            // 移動する前に行う計算
            _player.onGround = false;
            foreach (var block in _blocks)
            {
                if (_player.StandOn(block))
                {
                    _player.onGround = true;
                    _tmp_head = false;//仮
                    if (_player.VY > block.VY) _player.VY = block.VY;
                    if (_player.VX != block.VX) _player.VX = block.VX;
                }
                if (_player.HitHeadOn(block) && _player.VY < 0)
                {
                    _player.VY = 0;
                    _tmp_head = true;//仮
                }
            }
            if(!_player.onGround) _player.VX = 0;
            //foreach (var pblock in _pblocks)
            //{
            //    if (_player.StandOn(pblock) && !pblock.isWorking)
            //    {
            //        pblock.isWorking = true;
            //        //pblock.VY = -CellSize;
            //    }
            //}

            // 移動(重力によりめり込む)
            foreach (var pblock in _pblocks)
            {
                pblock.Update(dt);
            }
            _player.Update(dt);

            // 調整
            foreach (var block in _blocks)
            {
                _player.Adjust(block);
            }

            // 死亡判定
            //if (!_player.InWindow()) _player.Die();
            foreach (var enemy in _enemies)
            {
                if(_player.Intersects(enemy)) _player.Die();
            }

            // アイテム取得判定
            for (int i = _items.Count; --i >= 0;)
            {
                if (_player.Intersects(_items[i]))
                {
                    _player.Y -= _player.Height;
                    _player.Height *= 2;
                    _items.RemoveAt(i);
                }
            }

            // 画面のクリア
            ScreenClear();

            // 描画
            foreach (var enemy in _enemies)
            {
                enemy.Draw();
            }
            foreach (var item in _items)
            {
                item.Draw();
            }
            foreach (var block in _blocks)
            {
                block.Draw();
            }
            _player.Draw();

            // ゲーム画面外の描画
            _backButton.Draw();
            _resetButton.Draw();
        }

        private void ScreenClear()
        {
            GraphicsContext.Clear(Color.White);
            if (_player.onGround) GraphicsContext.FillRectangle(Brushes.BlueViolet, 1000, 200, 1200, 500); ;
            if (_tmp_head) GraphicsContext.FillRectangle(Brushes.GreenYellow, 1000, 100, 1200, 400);

        }

        private void GetProcess(ProcessfulObject pobj)
        {
            pobj.SetProcesses(new Process[] {
                            new Process((obj, dt) => { ; } , 5.0f),
                            //new Process((obj, dt) => { obj.VY = -CellSize; }, 4.0f),
                            //new Process((obj, dt) => { obj.VY = 0; } , 2.0f),
                            new Process((obj, dt) => { obj.VY = -CellSize; }, 0.1f),
                            new Process((obj, dt) => { ; } , 4.0f),
                            new Process((obj, dt) => { obj.VY = 0; } , 0.1f),
                            //new Process((obj, dt) => { obj.VX = -CellSize; }, 1.0f),
                            //new Process((obj, dt) => { obj.VX = 0; } , 2.0f),
                        });
        }
    }
}
