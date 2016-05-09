using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class EditScene : Scene
    {
        private MenuItem _backButton;
        private MenuItem _startButton;
        private List<MenuItem> _menuItem;
        private CodeBox _codebox;
        private Stage _stage;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _codebox = new CodeBox {Position = new Vector(400, 20)};
            _backButton = new MenuItem(Image.FromFile(@"image\back.png")) {
                Size = new Vector(50, 50),
                Position = new Vector(25, 500)
            };
            _startButton = new MenuItem(Image.FromFile(@"image\masato3.jpg")) {
                Size = new Vector(50, 50),
                Position = new Vector(75, 500)
            };
            _menuItem = new List<MenuItem> {_backButton, _startButton};

            _stage = new Stage();
        }

        public override void Update(float dt)
        {
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_startButton.Clicked) Scene.Push(new GameScene(_stage));
            if ((Input.Sp2.Pushed || Input.Back.Pushed) && !_codebox.IsFocused) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            _codebox.Update();

            if (Input.Control.Pressed)
            {
                if (Input.R.Pushed) _stage = Stage.Load();
                if (Input.S.Pushed) Stage.Save(_stage);
            }

            GraphicsContext.Clear(Color.White);
            foreach (var obj in _stage.Objects)
            {
                obj.Draw();
            }
            _codebox.Draw();
            _backButton.Draw();
            _startButton.Draw();
            GraphicsContext.DrawString(_codebox.GetString(), new Font("Arial", 12), Brushes.Black, new Rectangle(500, 300, 500, 300));
            
        }
    }
}
