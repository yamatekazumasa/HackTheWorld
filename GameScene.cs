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
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato.jpg");
        }

        public override void Update()
        {
//            Console.WriteLine("game scene.");
//            GraphicsContext.DrawImage(img, 0, 0, 192, 256);
//            System.IO.StreamReader sr = new System.IO.StreamReader("game.txt", System.Text.Encoding.GetEncoding("shift_jis"));
//            GraphicsContext.DrawString(sr.ReadToEnd(), new Font("ＭＳ ゴシック", 12), Brushes.Black, 192, 0);

            if (Input.Sp2.Pushed||Input.LeftButton.Pushed)
            {
                Scene.Pop();
            }
            
            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);

        }
    }
}
