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
        private MenuItem backButton = new MenuItem(Image.FromFile(@"image\back.png"));

        private ProcessfulObject pobj;
        private IEnumerator processes;
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato3.jpg");

            backButton.Size = new Vector(50, 50);
            backButton.Position = new Vector(25, 500);
            pobj = new ProcessfulObject();

            pobj.SetProcess( new Process[] {
                new Process(obj => { obj.Size = new Vector(10, 10); } , 60),
                new Process(obj => { obj.X += 1; }, 60),
                new Process(obj => { obj.Size = new Vector(30, 30); }, 60),
                new Process(obj => { obj.Size = new Vector(300, 300); }, 60)
            });

            processes = pobj.GetEnumerator();

        }

        public override void Update(float dt)
        {
            if (Input.Sp2.Pushed)

            {
                Scene.Pop();
            }
            if (backButton.Clicked) Scene.Pop();

            processes.MoveNext();

            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            pobj.Draw();
            backButton.Draw();
            
        }
    }
}
