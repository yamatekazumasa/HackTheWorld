﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MasatoScene1 : Scene
    {
        Image _img;
        Player _player;
        List<Block> _blocks;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            // playerの初期化
            _player = new Player(_img);

            // ブロックの初期化
            _blocks = new List<Block>();
            for (int iy = 0; iy < GridY; iy++)
            {
                for (int ix = 0; ix < GridX; ix++)
                {
                    if (Map[iy, ix] == 1)
                    {
                        _blocks.Add(new Block(Cell * ix, Cell * iy));
                    }
                }
            }

        }

        public override void Update(float dt)
        {
            if (Input.Sp2.Pushed||Input.LeftButton.Pushed)
            {
                Scene.Pop();
            }

            // 移動する前に行う計算
            foreach (var block in _blocks)
            {
                if (_player.StandOn(block) && _player.VY > 0)
                {
                    _player.VY = 0;
                    _player.onGround = true;// ジャンプの初速
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

            // 画面のクリア
            ScreenClear();

            // 描画
            _player.Draw();
            foreach (var block in _blocks)
            {
                block.Draw();
            }

        }

        private void ScreenClear()
        {
            GraphicsContext.Clear(Color.White);

        }
    }
}
