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
        private MenuItem backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private MenuItem resetButton = new MenuItem(Image.FromFile(@"image\reset.jpg"));
        Player _player;
        List<GameObject> _blocks;
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato1.jpg");

            backButton.Size = new Vector(50, 50);
            backButton.Position = new Vector(25, 600);
            resetButton.Size = new Vector(100,50);
            resetButton.Position = new Vector(100,600);
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



            if (backButton.Clicked(Input.Mouse.Position, Input.LeftButton.Pushed)) Scene.Pop();
            if (resetButton.Clicked(Input.Mouse.Position, Input.LeftButton.Pushed)) Startup(); ;
            backButton.Draw();
            resetButton.Draw(); ;
        }
    }
}
