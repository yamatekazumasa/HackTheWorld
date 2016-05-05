﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameScene : Scene
    {
        Image _img;
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private readonly List<MenuItem> _menuItem = new List<MenuItem>();

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato.jpg");
            _backButton.Size = new Vector(50, 50);
            _backButton.Position=new Vector(25, 500);
            _menuItem.Add(_backButton);
        }

        public override void Update(float dt)
        {
            if (Input.Sp2.Pushed) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();
            foreach (var button in _menuItem)
            {
                button.IsSelected = false;
                if (button.Contains(Input.Mouse.Position)) button.IsSelected = true;
            }
            if (_backButton.Clicked) Scene.Pop();



            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            _backButton.Draw();

        }
    }
}
