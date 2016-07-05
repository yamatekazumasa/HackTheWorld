using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// ステージを表示しながらゲーム中のオブジェクトのコードを編集するシーン
    /// </summary>
    class EditScene : Scene
    {
        private MenuItem _backButton;
        private MenuItem _startButton;
        private MenuItem _runButton;
        private List<MenuItem> _menuItem;
        private Stage _stage;
        private readonly CodeBox _codebox;

        public EditScene(Stage stage)
        {
            _stage = stage;
            if (stage.EditableObjects.Count == 0) _codebox = new CodeBox();
            else _codebox = new CodeBox(stage.EditableObjects[0]);
        }

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
        }

        public override void Update(float dt)
        {
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_backButton.Clicked) Scene.Pop();
            if (_startButton.Clicked) Scene.Push(new GameScene(_stage));
            if (_runButton.Clicked)
            {
                // 文字列を CodeParser.cs にもってく
                CodeParser.ConvertCodebox(_stage.EditableObjects[0].Code);
            }
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            if (Input.Control.Pressed)
            {
                if (Input.R.Pushed) _stage = Stage.Load("stage_1_1.json");
                if (Input.S.Pushed) _stage.Save();
            }

            foreach (var obj in _stage.EditableObjects)
            {
                if (obj.Clicked) _codebox.Focus(obj);
            }
            _codebox.Update();

            GraphicsContext.Clear(Color.White);
            _stage.Objects.ForEach(obj => obj.Draw());
            _codebox.Draw();

            _backButton.Draw();
            _startButton.Draw();
            _runButton.Draw( );
        }
    }
}
