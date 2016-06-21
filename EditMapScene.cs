using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class EditMapScene : Scene
    {
        private MenuItem _backButton;
        private MenuItem _startButton;
        private List<MenuItem> _menuItem;
        private MapEditor _mapEditor;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _backButton = new MenuItem(Image.FromFile(@"image\back.png"))
            {
                Size = new Vector(50, 50),
                Position = new Vector(25, 500)
            };
            _startButton = new MenuItem(Image.FromFile(@"image\masato3.jpg"))
            {
                Size = new Vector(50, 50),
                Position = new Vector(125, 500)
            };
         
            _menuItem = new List<MenuItem> { _backButton, _startButton };
            _mapEditor = new MapEditor();
        }

        public override void Update(float dt)
        {
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_startButton.Clicked)
            {
                Scene.Push(new GameScene(_mapEditor.GenerateStage()));
            }

            if (Input.Control.Pressed && Input.S.Pushed) _mapEditor.GenerateStage().Save();

            if ((Input.X.Pushed || Input.Back.Pushed)) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            _mapEditor.Update();

            GraphicsContext.Clear(Color.White);
            _mapEditor.Draw();

            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }
    }
}

