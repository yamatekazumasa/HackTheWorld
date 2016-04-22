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
        List<GameObject> _blocks;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            // playerの初期化
            _player = new Player(_img);
            // ブロックの初期化
            _blocks = new List<GameObject>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (Map[i, j] == 1)
                    {
                        _blocks.Add(new GameObject(ScreenWidth / 16 * j, ScreenHeight / 9 * i, 0, 0, ScreenWidth/16, ScreenHeight/9));
                    }
                }
            }
        }

        public override void Update()
        {
            if (Input.Sp2.Pushed||Input.LeftButton.Pushed)
            {
                Scene.Pop();
            }

            _player.Update();

            foreach (var block in _blocks)
            {
                if (_player.Intersects(block))
                {
                    _player.VY = 0;
                }
                _player.Adjust(block);
            }
            
            GraphicsContext.Clear(Color.White);
            _player.Draw();
            foreach (var block in _blocks)
            {
                block.Draw();
            }

        }
    }
}
