using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class ProcessTestScene : Scene
    {
        Image _img;
        private MenuItem _backButton;
        private List<MenuItem> _menuItem;
        private EditableObject _pobj;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato3.jpg");

            _backButton = new MenuItem(Image.FromFile(@"image\back.png")) {
                Size = new Vector(50, 50),
                Position = new Vector(25, 500)
            };
            _menuItem = new List<MenuItem> {_backButton};
            _pobj = new EditableObject();


            _pobj.SetProcesses( new [] {
                new Process((obj, dt) => { obj.Size = new Vector(10, 10); }),
                new Process((obj, dt) => { obj.X += 100*dt; }, 1.0f),
                new Process((obj, dt) => { obj.Size = new Vector(30, 30); }, 2.0f),
            });
            _pobj.Compile(new Stage());

        }

        public override void Update(float dt)
        {
            if (Input.X.Pushed || Input.Back.Pushed) Scene.Pop();
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_backButton.Clicked) Scene.Pop();

            _pobj.Update(dt);

            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_img, 0, 0);
            _pobj.Draw();
            _backButton.Draw();
            
        }
    }
}
