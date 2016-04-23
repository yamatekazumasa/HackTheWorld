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
        private CodeBox _box;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _box = new CodeBox();
            _box.Position = new Vector(400, 20);
            _img = Image.FromFile(@".\image\masato2.jpg");
            
        }

        public override void Update(float dt)
        {
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }

            _box.Update();
            
            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            _box.Draw();

        }
    }
}
