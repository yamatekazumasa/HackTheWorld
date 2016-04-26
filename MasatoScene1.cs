using System;
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
        List<Block1> _block1s;

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
            _block1s = new List<Block1>();
            for (int iy = 0; iy < CellNumY; iy++)
            {
                for (int ix = 0; ix < CellNumX; ix++)
                {
                    if (Map[iy, ix] == 2)
                    {
                        _blocks.Add(new Block(CellSize * ix, CellSize * iy));
                    }
                    if (Map[iy, ix] == 1)
                    {
                        var block1 = new Block1(CellSize * ix, CellSize * iy, 0, -CellSize);
                        _blocks.Add(block1);
                        _block1s.Add(block1);
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
            _player.onGround = false;
            foreach (var block in _blocks)
            {
                if (_player.StandOn(block))
                {
                    _player.onGround = true;
                    if (_player.VY > block.VY) _player.VY = block.VY;
                    if (block.work && !block.isWorking) block.isWorking = true;
                }
                if (_player.HitHeadOn(block) && _player.VY < 0)
                {
                    _player.VY = 0;
                }
            }

            // 移動
            _player.Update(dt);
            foreach (var block1 in _block1s)
            {
                block1.Update(dt);
            }

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
