using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class EditMapScene : Scene
    {
        private MenuItem _backButton;
        private MenuItem _startButton;
        private MenuItem _runButton;
        private List<MenuItem> _menuItem;
        private CodeBox _codebox;
        private Stage _stage;
        private int[,] Map2;//新しいマップの配列
        private string map2;//Map2の書き下し

        private MapObject _blockObject;
        private MapObject _nullObjject;
        private MapObject _enemyObject;
        private MapObject _itemObject;
        private List<MapObject> _selectedObject;
        private int _cursor;

        private class MapObject : GameObject
        {
            public int Type;
            public bool IsSelected = false;
            public MapObject(int type)
            {
                Size = new Vector(30, 30);
                this.Type = type;
            }

            public override void Draw()
            {
                switch (Type)
                {
                    case 0:
                        GraphicsContext.FillRectangle(Brushes.Aqua, MinX, MinY, Width, Height);
                        break;
                    case 1:
                        GraphicsContext.FillRectangle(Brushes.Brown, MinX, MinY, Width, Height);
                        break;
                    case 2:
                        GraphicsContext.FillRectangle(Brushes.Pink, MinX, MinY, Width, Height);
                        break;
                    case 3:
                        GraphicsContext.FillRectangle(Brushes.LightGreen, MinX, MinY, Width, Height);
                        break;

                }

            }

        }
        private List<MapObject> _mapObjects;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _codebox = new CodeBox { Position = new Vector(100, 20), Size = new Vector(160, 150) };
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
            _mapObjects = new List<MapObject>();
            _stage = new Stage();

            _nullObjject = new MapObject(0) { Position = new Vector(500, 50) };
            _blockObject = new MapObject(1) { Position = new Vector(550, 50) };
            _enemyObject = new MapObject(2) { Position = new Vector(600, 50) };
            _itemObject = new MapObject(3) { Position = new Vector(650, 50) };
            _selectedObject = new List<MapObject> { _nullObjject, _blockObject, _enemyObject, _itemObject };

            Map2 = new int[,]
            {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1}
            };

            _codebox.Current.MaxLine = Map2.GetLength(0);

            for (int i = 0; i < Map2.GetLength(0); i++)
            {
                for (int j = 0; j < Map2.GetLength(1); j++)
                {
                    _mapObjects.Add(new MapObject(Map2[i, j]) { Position = new Vector(j * 30, i * 30) + new Vector(500, 100) });
                    map2 = map2 + Map2[i, j].ToString();
                }
                map2 += "\n";
            }
            _codebox.SetString(map2);

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
                Map = Map2;
                Scene.Push(new GameScene(_stage));
            }
            if (_codebox.IsFocused)
            {
                string str = _codebox.GetString();
                for (int i = 0; i < Map2.GetLength(0); i++)
                {
                    for (int j = 0; ; j++)
                    {
                        char c = str.ToCharArray()[i * (16 + 1) + j];
                        if (c == '\n') break;
                        Map2[i, j] = int.Parse(c.ToString());
                        _mapObjects[i * Map2.GetLength(1) + j].Type = Map2[i, j];
                    }
                }
            }

            for (int i = 0; i < _selectedObject.Count; i++)
            {
                if (_selectedObject[i].Clicked) _cursor = i;
            }

            for (int i = 0; i < _mapObjects.Count; i++)
            {
                if (_mapObjects[i].Clicked)
                {
                    _mapObjects[i].Type = _cursor;
                    Map2[i / Map2.GetLength(1), i - i / Map2.GetLength(1) * Map2.GetLength(1)] = _cursor;

                    _codebox.Clear();
                    map2 = "";
                    for (int k = 0; k < Map2.GetLength(0); k++)
                    {
                        for (int j = 0; j < Map2.GetLength(1); j++)
                        {
                            map2 = map2 + Map2[k, j].ToString();
                        }
                        map2 += "\n";
                    }
                    _codebox.SetString(map2);
                }
            }


            if ((Input.X.Pushed || Input.Back.Pushed) && !_codebox.IsFocused) Scene.Pop();
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
            foreach (var item in _selectedObject)
            {
                item.Draw();
            }
            GraphicsContext.DrawRectangle(Pens.WhiteSmoke, _selectedObject[_cursor].MinX + 1, _selectedObject[_cursor].MinY + 1, _selectedObject[_cursor].Width - 3, _selectedObject[_cursor].Height - 3);
            foreach (var item in _menuItem)
            {
                item.Draw();
            }
            foreach (var item in _mapObjects)
            {
                item.Draw();
                if (item.Contains(Input.Mouse.Position)) GraphicsContext.DrawRectangle(Pens.WhiteSmoke, item.MinX + 1, item.MinY + 1, item.Width - 3, item.Height - 3);
            }
            _codebox.Draw();
        }
    }
}

