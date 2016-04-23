using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;


namespace HackTheWorld
{
    class TitleScene : Scene
    {
        private Image[] _menuImages;
        private MenuItem[] _menu;
        private int _cursor;
        private bool _focused;


        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _cursor = 0;
            _menuImages = new Image[10];
            _menu = new MenuItem[5];

            _menuImages[0] = Image.FromFile(@".\image\10.png");
            _menuImages[1] = Image.FromFile(@".\image\3.png");

            _menu[0] = new MenuItem(_menuImages[0], _menuImages[1]);
            _menu[0].Position = new Vector(100, 100);
            _menu[1] = new MenuItem(_menuImages[0], _menuImages[1]);
            _menu[1].Position = new Vector(100, 200);
            _menu[2] = new MenuItem(_menuImages[0], _menuImages[1]);
            _menu[2].Position = new Vector(100, 300);
            _menu[3] = new MenuItem(_menuImages[0], _menuImages[1]);
            _menu[3].Position = new Vector(100, 400);
            _menu[4] = new MenuItem(_menuImages[0], _menuImages[1]);
            _menu[4].Position = new Vector(100, 500);

        }

        public override void Update(float dt)
        {
//            Console.WriteLine("title scene.");
//            GraphicsContext.DrawImage(img, 0, 0, 192, 256);
//            System.IO.StreamReader sr = new System.IO.StreamReader("title.txt", System.Text.Encoding.GetEncoding("shift_jis"));
//
//            LinearGradientBrush b = new LinearGradientBrush(
//                GraphicsContext.VisibleClipBounds,
//                                        Color.White,
//                                        Color.Black,
//                                        LinearGradientMode.Horizontal);
//            GraphicsContext.DrawString(sr.ReadToEnd(), new Font("ＭＳ ゴシック", 50), b, 0, 256);

            _focused = false;


            if (_menu[0].Contains(Input.Mouse.Position))
            {
                _focused = true;
                _cursor = 0;
            }
            if (_menu[1].Contains(Input.Mouse.Position))
            {
                _focused = true;
                _cursor = 1;
            }
            if (_menu[2].Contains(Input.Mouse.Position))
            {
                _focused = true;
                _cursor = 2;
            }
            if (_menu[3].Contains(Input.Mouse.Position))
            {
                _focused = true;
                _cursor = 3;
            }
            if (_menu[4].Contains(Input.Mouse.Position))
            {
                _focused = true;
                _cursor = 4;
            }


            if (Input.Down.Pushed)
            {
                _cursor = (_cursor + 1) % 5;
            }

            if (Input.Up.Pushed)
            {
                _cursor = (_cursor + 4) % 5;
            }

            if (Input.Sp1.Pushed || (Input.LeftButton.Pushed && _focused))
            {
                switch (_cursor)
                {
                    case 0:
                        Scene.Push(new GameScene());
                        break;
                    case 1:
                        Scene.Push(new MasatoScene1());
                        break;
                    case 2:
                        Scene.Push(new MasatoScene2());
                        break;
                    case 3:
                        Scene.Push(new MasatoScene3());
                        break;
                    case 4:
                        Application.Exit();
                        break;
                }
            }

            if (Input.Sp2.Pushed)
            {
                _cursor = 4;
            }

            foreach (var item in _menu)
            {
                item._selected = false;
            }
            _menu[_cursor]._selected = true;



            GraphicsContext.Clear(Color.White);
            foreach (var item in _menu)
            {
                item.Draw();

            }



        }
    }
}
