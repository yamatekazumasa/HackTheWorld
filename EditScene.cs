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
        private MenuItem _runButton;
        private List<MenuItem> _menuItem;
        private Stage _stage;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _backButton = new MenuItem(Image.FromFile(@"image\back.png")) {
                Size = new Vector(50, 50),
                Position = new Vector(25, 500)
            };
            _startButton = new MenuItem(Image.FromFile(@"image\masato3.jpg")) {
                Size = new Vector(50, 50),
                Position = new Vector(75, 500)
            };
            _runButton = new MenuItem(Image.FromFile(@"image\run.PNG"))
            {
                Size = new Vector(75 , 75) ,
                Position = new Vector(125 , 500)
            };
            _menuItem = new List<MenuItem> {_backButton, _startButton,_runButton};

            _stage = Stage.CreateDemoStage();
        }

        public override void Update(float dt)
        {
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_startButton.Clicked) Scene.Push(new GameScene(_stage));
            if(_runButton.Clicked) 
            if (Input.X.Pushed || Input.Back.Pushed)
            {
                bool isFocused = false;
                foreach (var obj in _stage.EditableObjects)
                {
                    isFocused = isFocused || obj.IsFocused();
                }
                if (!isFocused) Scene.Pop();
            }
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            if (Input.Control.Pressed)
            {
                if (Input.R.Pushed)
                {
                    _stage = Stage.Load();
                }
                if (Input.S.Pushed)
                {
                    Stage.Save(_stage);
                }
            }

            foreach (var b in _stage.EditableObjects) b.Update(dt);

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
            _backButton.Draw();
            _startButton.Draw();
            _runButton.Draw( );
        }
    }
}
