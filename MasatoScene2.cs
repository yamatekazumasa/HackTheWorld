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
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@".\image\masato2.jpg");
        }

        public override void Update()
        {
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }
            
            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);

        }
    }
}
