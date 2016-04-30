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
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private readonly MenuItem _resetButton = new MenuItem(Image.FromFile(@"image\reset.jpg"));
        Player _player;
        List<GameObject> _blocks;
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato1.jpg");

            _backButton.Size = new Vector(50, 50);
            _backButton.Position = new Vector(25, 600);
            _resetButton.Size = new Vector(100,50);
            _resetButton.Position = new Vector(100,600);
            // playerの初期化
            _player = new Player(_img);

            // ブロックの初期化
            _blocks = new List<GameObject>();
            for (int iy = 0; iy < 9; iy++)
            {
                for (int ix = 0; ix < 16; ix++)
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
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }

            _player.Update(dt);

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



            if (_backButton.Clicked) Scene.Pop();
            if (_resetButton.Clicked) Startup(); ;
            _backButton.Draw();
            _resetButton.Draw(); ;
        }
    }
}
