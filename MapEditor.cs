using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        private readonly Palette[] _palettes;
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

            {
                StreamReader sr = new StreamReader(@".\palette.json", Encoding.GetEncoding("utf-8"));
                string json = sr.ReadToEnd();
                sr.Close();
                var tmp = JObject.Parse(json);
                foreach (var p in tmp["palettes"])
                {
                    Palette.ColorTable.Add(Type.GetType((string) p["type"]), new SolidBrush(Color.FromName((string) p["color"])));
                }
            }

            _palettes = new Palette[5];
            for (int i = 0; i < 5; i++)
            {
                _palettes[i] = new Palette(Palette.ColorTable.ElementAt(i).Key);
                _palettes[i].Position = new Vector(500 + 30 * i, 50);
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
            Stage s = new Stage();
            for (int i = 0; i < CellNumX; i++)
            {
                for (int j = 0; j < CellNumY; j++)
                {
                    var obj = _map[j, i];
                    if (obj == typeof(Block))
                    {
                        Block b = new Block(CellSize * i, CellSize * j);
                        s.Blocks.Add(b);
                        s.Objects.Add(b);
                    }
                    if (obj == typeof(Enemy))
                    {
                        Enemy e = new Enemy(CellSize * i, CellSize * j);
                        s.Enemies.Add(e);
                        s.Objects.Add(e);
                        break;
                    }
                    if (obj == typeof(Item))
                    {
                        Item item = new Item(CellSize * i + CellSize/4, CellSize * j + CellSize/2, ItemEffects.Bigger);
                        s.Items.Add(item);
                        s.Objects.Add(item);
                    }
                }
            }
            return s;
        }

        public void Update()
        {
            _cursorX = (Input.Mouse.X - (int)X) / 30;
            _cursorY = (Input.Mouse.Y - (int)Y) / 30;
            for (int i = 0; i < 5; i++)
            {
                if (_palettes[i].Clicked)
                {
                    _selected = i;
                }
            }

            if (!Contains(Input.Mouse.Position)) return;

            if (Clicked)
            {
                _map[_cursorY, _cursorX] = _palettes[_selected].Type;
            }
            if (RightClicked)
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
            for (int i = 0; i < 5; i++)
            {
                _palettes[i].Draw();
            }
            if (Contains(Input.Mouse.Position))
            {
                GraphicsContext.DrawRectangle(Pens.WhiteSmoke, X + _cursorX * 30, Y + _cursorY * 30, 30, 30);
            }
            GraphicsContext.DrawString("消しゴムは右クリック", new Font("Courier New", 12), Brushes.Black, X, MaxY + 20);
        }

        private class Palette : GameObject
        {
            public readonly Type Type;

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
