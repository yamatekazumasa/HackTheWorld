﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class CodeBox : GameObject
    {
        private List<StringBuilder> _code;
        private List<StringBuilder> _history;
        private int _prevLine;
        private int _prevCursor;
        private int _prevRows;
        private int _line;
        private int _cursor;
        private Tuple<int, int> _selectedBegin;
        private Tuple<int, int> _selectedEnd;
        private int _lineHeight;
        private int _rows;
        private int _cols;
        private string _clip;
        private bool _isDisplayed;
        private bool _isFocused;
        private Font _font;

        public bool IsFocused => _isFocused;

        public bool TextSelected => _selectedBegin != null && _selectedEnd != null;

        public CodeBox()
        {
            _line = 0;
            _cursor = 0;
            _cols = 40;
            _rows = 5;
            _lineHeight = 12;
            _code = new List<StringBuilder>();
            for (int i = 0; i < _rows; i++)
            {
                _code.Add(new StringBuilder());
            }
            _isDisplayed = true;
            _isFocused = false;
            _font = new Font("Courier New", 12);

            Width = 12 *_cols;
            Height = _lineHeight * _rows;

        }

        public void Update()
        {
            if (Input.Space.Pushed) _isFocused = true;

            if (Input.LeftButton.Pushed)
            {
                _isFocused = this.Contains(Input.Mouse.Position);
            }

            if (!_isFocused) return;

            if (Input.Up.Pushed)
            {
                _line--;
                if (_line < 0) _line = 0;
            }
            if (Input.Down.Pushed)
            {
                _line++;
                if (_line == _rows) _line = _rows - 1;
                if (_code[_line].Length < _cursor) _cursor = _code[_line].Length;
            }
            if (Input.Right.Pushed) _cursor++;
            if (Input.Left.Pushed) _cursor--;
            if (_cursor < 0)
            {
                if (_line == 0) _cursor = 0;
                else _cursor = _code[--_line].Length;
            }
            if (_cursor > _code[_line].Length)
            {
                if (_line == _rows - 1)
                {
                    _cursor = _code[_line].Length;
                }
                else
                {
                    _line++;
                    _cursor = 0;
                }
            }

            if (Input.Enter.Pushed)
            {
                Historize();
                char[] c = new char[_code[_line].Length - _cursor];
                StringBuilder str = new StringBuilder();
                _code[_line].CopyTo(_cursor, c, 0, _code[_line].Length - _cursor);
                _code[_line].Remove(_cursor, _code[_line].Length - _cursor);
                str.Insert(0, c);

                _cursor = 0;
                _code.Insert(++_line, str);
                _rows++;
            }
            if (Input.Back.Pushed)
            {
                Historize();
                if (_cursor > 0)
                {
                    _code[_line].Remove(--_cursor, 1);
                }
                else if (_line > 0)
                {
                    _cursor = _code[_line - 1].Length;
                    _code[_line - 1].Insert(_code[_line - 1].Length, _code[_line].ToString());
                    _code.RemoveAt(_line);
                    _line--;
                    _rows--;
                }
            }
            if (Input.Delete.Pushed)
            {
                Historize();
                if (_cursor < _code[_line].Length)
                {
                    _code[_line].Remove(_cursor, 1);
                }
                else if (_line < _rows - 1)
                {
                    _code[_line].Insert(_code[_line].Length, _code[_line + 1].ToString());
                    _code.RemoveAt(_line + 1);
                    _rows--;
                }
            }

            if (_line < 0) _line = 0;
            if (_line == _rows) _line = _rows - 1;
            if (_cursor < 0)
            {
                if (_line == 0) _cursor = 0;
                else            _cursor = _code[--_line].Length;
            }
            if (_cursor > _code[_line].Length)
            {
                if (_line == _rows - 1)
                {
                    _cursor = _code[_line].Length;
                }
                else
                {
                    _line++;
                    _cursor = 0;
                }
            }

            if (Input.Tab.Pushed)
            {
                Historize();
                _code[_line].Insert(_cursor, "  ");
                _cursor += 2;
            }

            if (Input.Shift.Pressed)
            {
                if (_selectedBegin == null) _selectedBegin = Tuple.Create(_line, _cursor);
                if (Input.Up.Pushed || Input.Down.Pushed || Input.Right.Pushed || Input.Left.Pushed)
                {
                    _selectedEnd = Tuple.Create(_line, _cursor);
                }
            }
            else if(_selectedEnd != null && (_line != _selectedEnd.Item1 || _cursor != _selectedEnd.Item2))
            {
                _selectedBegin = null;
                _selectedEnd = null;
            }

            if (Input.Control.Pressed)
            {
                if (Input.Sp1.Pushed)
                {
                    Dehistorize();
                }
            }

            Height = _lineHeight * _rows;
        }

        public string GetString()
        {
            string str = "";
            for (int i = 0; i < _rows; i++)
            {
                str += _code[i] + "\n";
            }
            return str;
        }

        public void Insert(char c)
        {
            if (_isFocused && !Input.Control.Pressed)
            {
                Historize();
                _code[_line].Insert(_cursor++, c);
            }
            Input.KeyBoard.Clear();
        }

        public void Historize()
        {
            _history = new List<StringBuilder>(_rows);
            for (int i = 0; i < _rows; i++) _history.Add(new StringBuilder(_code[i].ToString()));
            _prevLine = _line;
            _prevCursor = _cursor;
            _prevRows = _rows;
        }

        public void Dehistorize()
        {
            _code = new List<StringBuilder>(_history);
            _line = _prevLine;
            _cursor = _prevCursor;
            _rows = _prevRows;
        }

        public override void Draw()
        {
            if (_isDisplayed)
            {
                if (_isFocused) GraphicsContext.FillRectangle(Brushes.White, this);
                else GraphicsContext.FillRectangle(Brushes.LightGray, this);
                GraphicsContext.DrawRectangle(Pens.DarkGray, this);

                // 選択範囲の描画
                if (TextSelected)
                {
                    if (_selectedBegin.Item1 == _selectedEnd.Item1)
                    {
                        if (_selectedBegin.Item2 < _selectedEnd.Item2)
                        {
                            int startX = (int) MinX + _selectedBegin.Item2*10 + 2;
                            int startY = (int)MinY + _selectedBegin.Item1* _lineHeight;
                            GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, (_selectedEnd.Item2 - _selectedBegin.Item2) * 10, _lineHeight + 5);
                        }
                        else
                        {
                            int startX = (int)MinX + _selectedEnd.Item2 * 10 + 2;
                            int startY = (int)MinY + _selectedEnd.Item1 * _lineHeight;
                            GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, (_selectedBegin.Item2 - _selectedEnd.Item2) * 10, _lineHeight + 5);
                        }
                    }
                    else if (_selectedBegin.Item1 < _selectedEnd.Item1)
                    {
                        int startX = (int) MinX + _selectedBegin.Item2*10 + 2;
                        int startY = (int) MinY + _selectedBegin.Item1* _lineHeight;
                        int endX = _selectedEnd.Item2*10 + 2;
                        int endY = (int) MinY + _selectedEnd.Item1* _lineHeight;
                        GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, MaxX - startX, _lineHeight + 5);
                        for (int i = _selectedBegin.Item1+1; i < _selectedEnd.Item1; i++)
                        {
                            GraphicsContext.FillRectangle(Brushes.LightBlue, MinX, MinY + i * _lineHeight, Width, _lineHeight + 5);
                        }
                        GraphicsContext.FillRectangle(Brushes.LightBlue, MinX, endY, endX, _lineHeight + 5);
                    }
                    else
                    {
                        int startX = (int)MinX + _selectedEnd.Item2 * 10 + 2;
                        int startY = (int)MinY + _selectedEnd.Item1 * _lineHeight;
                        int endX = _selectedBegin.Item2 * 10 + 2;
                        int endY = (int)MinY + _selectedBegin.Item1 * _lineHeight;
                        GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, MaxX - startX, _lineHeight + 5);
                        for (int i = _selectedEnd.Item1 + 1; i < _selectedBegin.Item1; i++)
                        {
                            GraphicsContext.FillRectangle(Brushes.LightBlue, MinX, MinY + i * _lineHeight, Width, _lineHeight + 5);
                        }
                        GraphicsContext.FillRectangle(Brushes.LightBlue, MinX, endY, endX, _lineHeight + 5);

                    }
                }
                for (int i = 0; i < _rows; i++)
                {
                    GraphicsContext.DrawString(_code[i].ToString(), _font, Brushes.Black, X, Y + i * _lineHeight);
                }
                GraphicsContext.DrawLine(Pens.Black, X + 10 * _cursor + 2, Y + _lineHeight * _line + 2, X + 10 * _cursor + 2, Y + _lineHeight * (_line + 1) + 2);
            }
        }


    }
}
