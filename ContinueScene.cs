using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;
using System.Drawing;


namespace HackTheWorld
{
    class ContinueScene : Scene
    {
        private readonly MenuItem _continueButton = new MenuItem(Image.FromFile(@"image\continue.bmp"), Image.FromFile(@"image\continue1.bmp"));
        private readonly MenuItem _closeButton = new MenuItem(Image.FromFile(@"image\close1.bmp"), Image.FromFile(@"image\close.bmp"));
        List<MenuItem> menuItem = new List<MenuItem>();

        //コントロールの外観を描画するBitmapの作成
        Bitmap bmp = new Bitmap(ScreenWidth, ScreenHeight);
        public override void Cleanup()
        {
        }
        public override void Startup()
        {
            _continueButton.Size = new Vector(400, 100);
            _continueButton.Position = new Vector(400, 200);
            _closeButton.Size = new Vector(400, 100);
            _closeButton.Position = new Vector(400, 400);
            menuItem.Add(_continueButton); menuItem.Add(_closeButton);
        }
        public override void Update(float dt)
        {
            foreach (var button in menuItem)
            {
                button.IsSelected = false;
                if (button.Contains(Input.Mouse.Position)) button.IsSelected = true;
            }
            if (_continueButton.Clicked)
            {
                Scene.Pop();
                Scene.Pop();
                Scene.Push(new MasatoScene1());
            }
            if (_closeButton.Clicked)
            {
                Scene.Pop();
                Scene.Pop();
            }
            foreach (var item in menuItem)
            {
                item.Draw();
            }
        }
    }
}