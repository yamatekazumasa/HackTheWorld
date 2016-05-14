using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;
using System.Drawing;


namespace HackTheWorld
{
    class PauseScene : Scene
    {
        private int _cursor;
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
                Position = new Vector(400, 200)
            };
            _closeButton = new MenuItem(Image.FromFile(@"image\close1.bmp"), Image.FromFile(@"image\close.bmp"))
            {
                Size = new Vector(400, 100),
                Position = new Vector(400, 400)
            };
            _menuItem = new List<MenuItem> {_continueButton, _closeButton};
        }
        public override void Update(float dt)

        {
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
                        break;
                    case 1:
                        Scene.Current = new TitleScene();
                        break;
                }
            }
            if (_continueButton.Clicked)
            {
                Scene.Pop();
            }
            if (_closeButton.Clicked)
            {
                Scene.Current = new TitleScene();
            }
            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }
    }
}
