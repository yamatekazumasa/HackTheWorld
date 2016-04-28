using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MasatoScene2 : Scene
    {
        Image _img;
        private MenuItem backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private MenuItem masato3Button = new MenuItem(Image.FromFile(@"image\masato3.jpg"));
        private CodeBox _box;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _box = new CodeBox();
            _box.Position = new Vector(400, 20);
            _img = Image.FromFile(@".\image\masato2.jpg");

            backButton.Size = new Vector(50, 50);
            backButton.Position = new Vector(25, 500);

            masato3Button.Size = new Vector(50, 50);
            masato3Button.Position = new Vector(75, 500);
        }

        public override void Update(float dt)
        {
            if (backButton.Clicked(Input.Mouse.Position, Input.LeftButton.Pushed)) Scene.Pop();
            if (masato3Button.Clicked(Input.Mouse.Position, Input.LeftButton.Pushed))Scene.Push(new MasatoScene3()) ;

            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }
            _box.Update();
            
            
            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            _box.Draw();
            backButton.Draw();
            masato3Button.Draw();
        }
    }
}
