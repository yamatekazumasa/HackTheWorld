using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// マップエディタ
    /// </summary>
    class MapEditor : GameObject
    {
        private Palette[] _palettes;
        private int[,] _map;
        private int _cursorX;
        private int _cursorY;
        private int _selected;

        public MapEditor()
        {
            _palettes = new Palette[5];
            for (int i = 0; i < 5; i++)
            {
                _palettes[i] = new Palette((ObjectType)i);
                _palettes[i].Position = new Vector(500 + 50*i, 50);
            }
            _cursorX = -1;
            _cursorY = -1;
            X = 500;
            Y = 100;
            W = CellNumX*30;
            H = CellNumY*30;

            _map = new[,] {
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
                    switch ((ObjectType)_map[j, i])
                    {
                        case ObjectType.Block:
                            {
                                Block b = new Block(CellSize * i, CellSize * j);
                                s.Blocks.Add(b);
                                s.Objects.Add(b);
                                break;
                            }
                        case ObjectType.Enemy:
                            {
                                Enemy e = new Enemy(CellSize * i, CellSize * j);
                                s.Enemies.Add(e);
                                s.Objects.Add(e);
                                break;
                            }
                        case ObjectType.Item:
                            {
                                Item obj = new Item(CellSize * i, CellSize * j, ItemEffects.Bigger);
                                s.Items.Add(obj);
                                s.Objects.Add(obj);
                                break;
                            }
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
                _map[_cursorY, _cursorX] = (int)_palettes[_selected].Type;
            }

        }

        public override void Draw()
        {
            for (int i = 0; i < CellNumX; i++)
            {
                for (int j = 0; j < CellNumY; j++)
                {
                    GraphicsContext.FillRectangle(Palette.ColorOf((ObjectType)_map[j, i]), X + i*30, Y + j*30, 30, 30);
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

        }

        private class Palette : GameObject
        {
            public ObjectType Type;

            public static Brush ColorOf(ObjectType type)
            {
                switch (type)
                {
                    case ObjectType.Block: return Brushes.Brown;
                    case ObjectType.Enemy: return Brushes.Pink;
                    case ObjectType.Item:  return Brushes.LightGreen;
                    default:               return Brushes.Aqua;
                }
            }

            public Palette(ObjectType type)
            {
                Size = new Vector(30, 30);
                Type = type;
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
