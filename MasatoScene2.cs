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
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private readonly MenuItem _masato3Button = new MenuItem(Image.FromFile(@"image\masato3.jpg"));
        private CodeBox _box;
        List<MenuItem> menuItem = new List<MenuItem>();

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _box = new CodeBox();
            _box.Position = new Vector(400, 20);
            _img = Image.FromFile(@".\image\masato2.jpg");

            _backButton.Size = new Vector(50, 50);
            _backButton.Position = new Vector(25, 500);
            _masato3Button.Size = new Vector(50, 50);
            _masato3Button.Position = new Vector(75, 500);
            menuItem.Add(_backButton);menuItem.Add(_masato3Button);
        }

        public override void Update(float dt)
        {
            foreach (var button in menuItem)
            {
                button.IsSelected = false;
                if (button.Contains(Input.Mouse.Position)) button.IsSelected = true;
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_masato3Button.Clicked) Scene.Push(new MasatoScene3());
            if (Input.Sp2.Pushed && !_box.IsFocused) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();


            _box.Update();


            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            _box.Draw();
            _backButton.Draw();
            _masato3Button.Draw();
            GraphicsContext.DrawString(_box.GetString(), new Font("Arial", 12), Brushes.Black, new Rectangle(500, 300, 500, 300));
            
        }
    }
}
