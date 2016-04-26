using System;
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
        private readonly List<Keys> _availableKeys;
        private readonly List<Keys> _operationKeys;
        private int _line;
        private int _cursor;
        private int _selectedBegin;
        private int _selectedEnd;
        private int _lineHeight;
        private int _rows;
        private int _cols;
        private string _clipped;
        private bool _isDisplayed;
        private bool _isFocused;
        private Font _font;

        public CodeBox()
        {
            _line = 0;
            _cursor = 0;
            _cols = 40;
            _rows = 10;
            _lineHeight = 12;
            _code = new List<StringBuilder>();
            for (int i = 0; i < _rows; i++)
            {
                _code.Add(new StringBuilder());
            }
            _isDisplayed = true;
            _isFocused = true;
            _font = new Font("Courier New", 12);

            Width = 12 *_cols;
            Height = _lineHeight * _rows;


            _availableKeys = new List<Keys>
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
            };

            foreach (var key in _availableKeys)
            {
                Input.KeyBoard.Append(key, 0);
            }

        }

        public void Update()
        {

            if (Input.LeftButton.Pushed)
            {
                if (this.Contains(Input.Mouse.Position)) _isFocused = true;
                else _isFocused = false;
            }

            if (!_isFocused) return;

            if (Input.Up.Pushed) _line--;
            if (Input.Down.Pushed) _line++;
            if (Input.Right.Pushed) _cursor++;
            if (Input.Left.Pushed) _cursor--;
            if (Input.Enter.Pushed)
            {
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
            if (_cursor < 0 && _line != 0)
            {
                _cursor = _code[--_line].Length;
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

            if (Input.Space.Pushed) _code[_line].Insert(_cursor++, " ");
            if (Input.Tab.Pushed)
            {
                _code[_line].Insert(_cursor, "    ");
                _cursor += 4;
            }

            if (Input.Control.Pressed)
            {
                if (Input.Sp3.Pushed)
                {

                }
            }

            foreach (var key in _availableKeys)
            {
                if (Input.KeyBoard.Pushed(key))
                {
                    if (Input.Shift.Pressed) _code[_line].Insert(_cursor++, key.ToString().ToUpper());
                    else                     _code[_line].Insert(_cursor++, key.ToString().ToLower());
                }
            }
        }

        public override void Draw()
        {
            if (_isDisplayed)
            {
                if (_isFocused) GraphicsContext.FillRectangle(Brushes.Yellow, this);
                else GraphicsContext.FillRectangle(Brushes.DarkOrange, this);
                for (int i = 0; i < _rows; i++)
                {
                    GraphicsContext.DrawString(_code[i].ToString(), _font, Brushes.Black, X, Y + i * _lineHeight);
                }
                GraphicsContext.DrawLine(Pens.Black, X + 10 * _cursor, Y + _lineHeight * _line, X + 10 * _cursor, Y + _lineHeight * (_line + 1));
            }
        }


    }
}
