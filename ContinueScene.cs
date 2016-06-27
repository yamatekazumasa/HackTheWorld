using System.Collections.Generic;
using static HackTheWorld.Constants;
using System.Drawing;
using System.Windows.Forms;


namespace HackTheWorld
{
    /// <summary>
    /// コンティニュー画面
    /// </summary>
    class ContinueScene : Scene
    {
        private int _cursor;

        //画像を読み込む
        readonly Bitmap _bmp = new Bitmap(@"image\gameover.bmp");

        private MenuItem _continueButton;
        private MenuItem _closeButton;
        private List<MenuItem> _menuItem;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _continueButton = new MenuItem(Image.FromFile(@"image\continue.bmp"), Image.FromFile(@"image\continue1.bmp"))
            {
                Size = new Vector(400, 100),
                Position = new Vector(800, 200)
            };
            _closeButton = new MenuItem(Image.FromFile(@"image\close1.bmp"), Image.FromFile(@"image\close.bmp"))
            {
                Size = new Vector(400, 100),
                Position = new Vector(800, 300)
            };
            _menuItem = new List<MenuItem> {_continueButton, _closeButton};
            _cursor = 0;

        }

        public override void Update(float dt)
        {
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            //背景を透明にする
            _bmp.MakeTransparent();
            GraphicsContext.DrawImage(_bmp,  0, 0);


            if (Input.Down.Pushed || Input.Up.Pushed)
            {
                _cursor = (_cursor + 1) % 2;
            }

            for (int i = 0; i < _menuItem.Count; i++)
            {
                _menuItem[i].IsSelected = false;
                if (_cursor == i) _menuItem[i].IsSelected = true;
                if (_menuItem[i].Contains(Input.Mouse.Position))
                {
                    _cursor = -1;
                    _menuItem[i].IsSelected = true;
                }
            }
            //Zを押したときの処理
            if (Input.Z.Pushed)
            {
                switch (_cursor)
                {
                    case -1:
                        break;
                    case 0:
                        Scene.Pop();
                        Scene.Current.Startup();
                        break;
                    case 1:
                        Scene.Current = new TitleScene();
                        break;
                }
            }
            //クリックしたときの処理
            if (_continueButton.Clicked)
            {
                Scene.Pop();
                Scene.Current.Startup();
            }
            
            if (_closeButton.Clicked)
            {
                Scene.Current = new TitleScene();
            }

            //背景を透明にする
            _bmp.MakeTransparent();
            GraphicsContext.DrawImage(_bmp, 0, 0);

            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }
    }
}