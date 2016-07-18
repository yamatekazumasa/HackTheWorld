using System;
using System.Drawing;
using System.IO;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class StageSelectScene : Scene
    {
        private Font _font;
        private int _cursor;
        private string[] _files;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _cursor = 0;
            _font = new Font("Courier New", 12);
            _files = Directory.GetFiles(@".\stage\", "*.json", SearchOption.TopDirectoryOnly);
        }

        public override void Update(float dt)
        {
            int length = _files.Length;
            if (Input.Up.Pushed)   _cursor = (_cursor-1) % length;
            if (Input.Down.Pushed) _cursor = (_cursor+1) % length;

            if (Input.Z.Pushed) Scene.Push(new EditScene(Stage.Load(Path.GetFileName(_files[_cursor]))));

            GraphicsContext.Clear(Color.White);
            for (int i = 0; i < length; i++)
            {
                GraphicsContext.DrawString(_files[i], _font, i == _cursor ? Brushes.Yellow : Brushes.Black, 150, 200 + 20*i);
            }
        }
    }
}
