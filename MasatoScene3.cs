using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MasatoScene3 : Scene
    {
        Image _img;
        private Button backButton = new Button(Image.FromFile(@"image\back.png"));

        private ProcessfulObject pobj;
        private IEnumerator processes;
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato3.jpg");
            backButton.SetSize(50, 50); backButton.SetPosition(25, 500);

            pobj = new ProcessfulObject(new Process[4] {
                new Process(obj => obj.SetSize(10, 10), 60),
                new Process(obj => { }, 60),
                new Process(obj => obj.SetSize(30, 30), 60),
                new Process(obj => obj.SetSize(300, 300), 60)
            });
            processes = pobj.GetEnumerator();

        }

        public override void Update()
        {
            if (Input.Sp2.Pushed)

            {
                Scene.Pop();
            }
            if (backButton.clicked(Input.mp.position, Input.MouseLeft.Pushed)) Scene.Pop();

            processes.MoveNext();

            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            pobj.Draw();
            backButton.Draw();

        }
    }
}
