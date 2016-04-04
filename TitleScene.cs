using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class TitleScene : Scene
    {
        Image img;
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            img = System.Drawing.Image.FromFile(@"inouemasatosennsei.png");
        }

        public override void Update()
        {
            Console.WriteLine("title scene.");
            GraphicsContext.DrawImage(img, 0, 0,192,256);
            System.IO.StreamReader sr = new System.IO.StreamReader("title.txt", System.Text.Encoding.GetEncoding("shift_jis"));

            LinearGradientBrush b = new LinearGradientBrush(
                GraphicsContext.VisibleClipBounds,
                                        Color.White,
                                        Color.Black,
                                        LinearGradientMode.Horizontal);
            GraphicsContext.DrawString(sr.ReadToEnd(), new Font("ＭＳ ゴシック", 50), b, 0, 256);

        }
    }
}
