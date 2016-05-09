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
            foreach (var button in _menuItem)
            {
                button.IsSelected = false;
                if (button.Contains(Input.Mouse.Position)) button.IsSelected = true;
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
