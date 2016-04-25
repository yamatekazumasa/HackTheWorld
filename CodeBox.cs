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
        private StringBuilder _str;
        private readonly List<Keys> _availableKeys;
        private int _cursor;
        private int _selectedBegin;
        private int _selectedEnd;
        private int _lineHeight;
        private int _cols;
        private string _clipped;
        private bool _isDisplayed;
        private bool _isFocused;
        private Font _font;

        public CodeBox()
        {
            _cursor = 0;
            _cols = 40;
            _lineHeight = 10;
            _str = new StringBuilder(80);
            _isDisplayed = true;
            _isFocused = true;
            _font = new Font("ＭＳ ゴシック", 12);

            Width = 10*_cols;
            Height = _lineHeight * 20;


            _availableKeys = new List<Keys>
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
                Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
                Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.NumPad0
            };

            foreach(var key in _availableKeys)
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

            if (Input.Up.Pushed) _cursor -= _cols;
            if (Input.Down.Pushed) _cursor += _cols;
            if (Input.Right.Pushed) _cursor++;
            if (Input.Left.Pushed) _cursor--;
            if (_cursor < 0) _cursor = 0;
            if (_cursor > _str.Length) _cursor = _str.Length;

            if (Input.Space.Pushed) _str.Insert(_cursor++, " ");
            if (Input.Enter.Pushed) _str.Insert(_cursor++, "\n");
            if (Input.Tab.Pushed)
            {
                _str.Insert(_cursor, "    ");
                _cursor += 4;
            }

            if (Input.Back.Pushed && _cursor > 0) _str.Remove(--_cursor, 1);
            if (Input.Delete.Pushed && _cursor < _str.Length) _str.Remove(_cursor, 1);

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
                    if (Input.Shift.Pressed) _str.Insert(_cursor++, key.ToString().ToUpper());
                    else                     _str.Insert(_cursor++, key.ToString().ToLower());
                }
            }
        }

        public override void Draw()
        {
            if (_isDisplayed)
            {
                if (_isFocused) GraphicsContext.FillRectangle(Brushes.Yellow, this);
                else GraphicsContext.FillRectangle(Brushes.DarkOrange, this);
                GraphicsContext.DrawString(_str.ToString(), _font, Brushes.Black, (Rectangle)this);
            }
        }


    }
}
