using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using static HackTheWorld.Constants;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;

namespace HackTheWorld
{
    class Null { }

    /// <summary>
    /// マップエディタ
    /// </summary>
    class MapEditor : GameObject
    {
        private readonly List<Palette> _palettes;
        private readonly Type[,] _map;
        private int _cursorX;
        private int _cursorY;
        private int _selected;

        public MapEditor()
        {
            _cursorX = -1;
            _cursorY = -1;
            X = 500;
            Y = 100;
            W = CellNumX*30;
            H = CellNumY*30;

            StreamReader sr = new StreamReader(@".\palette.json", Encoding.GetEncoding("utf-8"));
            string json = sr.ReadToEnd();
            sr.Close();
            var tmp = JObject.Parse(json);
            foreach (var p in tmp["palettes"])
            {
                if (Type.GetType((string)p["type"]) != null && !Palette.ColorTable.ContainsKey(Type.GetType((string)p["type"])))
                {
                    Palette.ColorTable.Add(Type.GetType((string)p["type"]), new SolidBrush(Color.FromName((string)p["color"])));
                }
            }

            _palettes = new List<Palette>();
            for (int i = 0; i < Palette.ColorTable.Count; i++)
            {
                Palette p = new Palette(Palette.ColorTable.ElementAt(i).Key) {
                    Position = new Vector(500 + 30*(i%16), 40 + i/16*30)
                };
                _palettes.Add(p);
            }

            _map = new Type[CellNumY, CellNumX];
            for (int i = 0; i < CellNumX; i++)
            {
                for (int j = 0; j < CellNumY; j++)
                {
                    if (j == CellNumY - 1) _map[j, i] = typeof(Block);
                    else _map[j, i] = typeof(Null);
                }
            }

        }

        /// <summary>
        /// 編集しているマップからステージを生成する。
        /// </summary>
        public Stage GenerateStage()
        {
            int width = _map.GetLength(1);
            int height = _map.GetLength(0);
            Stage s = new Stage(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var obj = _map[j, i];
                    if (obj == typeof(Player))
                    {
                        Player p = new Player(CellSize * i, CellSize * j);
                        s.Player = p;
                        s.Objects.Add(p);
                        continue;
                    }
                    if (obj == typeof(Block))
                    {
                        Block b = new Block(CellSize * i, CellSize * j);
                        s.Blocks.Add(b);
                        s.Objects.Add(b);
                        continue;
                    }
                    if (obj == typeof(Enemy))
                    {
                        Enemy e = new Enemy(CellSize * i, CellSize * j);
                        s.Enemies.Add(e);
                        s.Objects.Add(e);
                        continue;
                    }
                    if (obj == typeof(Item))
                    {
                        Item item = new Item(CellSize * i + CellSize/4, CellSize * j + CellSize/2, ItemEffects.Bigger);
                        s.Items.Add(item);
                        s.Objects.Add(item);
                        continue;
                    }
                }
            }
            return s;
        }

        public void Update()
        {
            _cursorX = (Input.Mouse.X - (int)X) / 30;
            _cursorY = (Input.Mouse.Y - (int)Y) / 30;
            for (int i = 0; i < _palettes.Count; i++)
            {
                if (_palettes[i].Clicked)
                {
                    _selected = i;
                }
            }

            if (!Contains(Input.Mouse.Position)) return;

            if (Input.Mouse.Left.Pressed)
            {
                _map[_cursorY, _cursorX] = _palettes[_selected].Type;
            }
            if (Input.Mouse.Right.Pressed)
            {
                _map[_cursorY, _cursorX] = typeof(Null);
            }

        }

        public override void Draw()
        {
            for (int i = 0; i < CellNumX; i++)
            {
                for (int j = 0; j < CellNumY; j++)
                {
                    GraphicsContext.FillRectangle(Palette.ColorOf(_map[j, i]), X + i*30, Y + j*30, 30, 30);
                }
            }
            foreach(var p in _palettes)
            {
                p.Draw();
            }
            if (Contains(Input.Mouse.Position))
            {
                GraphicsContext.DrawRectangle(Pens.WhiteSmoke, X + _cursorX * 30, Y + _cursorY * 30, 30, 30);
            }
            GraphicsContext.DrawString("消しゴムは右クリック", new Font("Courier New", 12), Brushes.Black, X, MaxY + 20);
        }

        /// <summary>
        /// 一つ一つのカラーパレット。
        /// </summary>
        private class Palette : GameObject
        {
            public readonly Type Type;

            /// <summary>
            /// 型と色の対応付けを保存している。
            /// </summary>
            public static readonly Dictionary<Type, Brush> ColorTable = new Dictionary<Type, Brush>();

            public Palette(Type type)
            {
                Size = new Vector(30, 30);
                Type = type;
            }

            public static Brush ColorOf(Type t)
            {
                return ColorTable[t];
            }

            public override void Draw()
            {
                GraphicsContext.FillRectangle(ColorOf(Type), this);
                if (Contains(Input.Mouse.Position))
                {
                    GraphicsContext.DrawRectangle(Pens.Black, this);
                }
            }

        }

    }
}
