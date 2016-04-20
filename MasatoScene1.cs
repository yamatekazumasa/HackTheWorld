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
        private Button backButton = new Button(Image.FromFile(@"image\back.png"));
        
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            backButton.SetSize(50, 50);backButton.SetPosition(25,500 );
        }
       
        public override void Update()
        {
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }



            if (backButton.clicked(Input.mp.position, Input.MouseLeft.Pushed)) Scene.Pop();



                GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            backButton.Draw();
        }
    }
}
