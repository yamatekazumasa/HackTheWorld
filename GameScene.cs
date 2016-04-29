using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameScene : Scene
    {
        Image _img;
        private MenuItem backButton = new MenuItem(Image.FromFile(@"image\back.png"));

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato.jpg");
            backButton.Size = new Vector(50, 50);
            backButton.Position=new Vector(25, 500);

        }

        public override void Update(float dt)
        {
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }
            if (backButton.Clicked) Scene.Pop();



            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            backButton.Draw();

        }
    }
}
