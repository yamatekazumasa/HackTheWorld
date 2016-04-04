using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameScene : Scene
    {
        Image img;
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            img = System.Drawing.Image.FromFile(@"inoue.jpg");
        }

        public override void Update()
        {
            Console.WriteLine("game scene.");
            GraphicsContext.DrawImage(img, 0, 0, 192, 256);
            System.IO.StreamReader sr = new System.IO.StreamReader("game.txt", System.Text.Encoding.GetEncoding("shift_jis"));
            GraphicsContext.DrawString(sr.ReadToEnd(), new Font("ＭＳ ゴシック", 12), Brushes.Black, 192, 0);

        }
    }
}
