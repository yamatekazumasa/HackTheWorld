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
        class State
        {
            private static State[] _state = new State[50];
            private static int _origin;
            private static int _current;
            private static readonly int _length = 50;

            public int Line { get; set; }
            public int Cursor { get; set; }
            public int MaxLine { get; set; }
            public List<StringBuilder> Text { get; }

            public static State Current
            {
                get { return _state[_current]; }
                set { _state[_current] = value; }
            }

            public State(int line, int cursor, int maxLine)
            {
                Line = line;
                Cursor = cursor;
                MaxLine = maxLine;
                Text = new List<StringBuilder>(maxLine);
                for (int i = 0; i < maxLine; i++)
                {
                    Text.Add(new StringBuilder());
                }
            }

            public static void Record(State s)
            {
                _current = (_current + 1) % _length;
                _origin = _current;
                _state[_current] = new State(s.Line, s.Cursor, s.MaxLine);
                for (int i = 0; i < s.MaxLine; i++)
                {
                    _state[_current].Text[i] = new StringBuilder(s.Text[i].ToString());
                }
            }

            public static void Undo()
            {
                if (_current > 0) _current = (_current + _length - 1)%_length;
            }

            public static void Redo()
            {
                if (_state[_current + 1] != null && _current < _origin) _current = (_current + 1) % _length;
            }

        }

        private Tuple<int, int> _selectedBegin;
        private Tuple<int, int> _selectedEnd;
        private int _lineHeight;
        private int _cols;
        private string _clip;
        private bool _isDisplayed;
        private bool _isFocused;
        private readonly Font _font;
        private int frame;

        public bool IsFocused => _isFocused;

        public bool TextSelected => _selectedBegin != null && _selectedEnd != null;

        public CodeBox()
        {
            _cols = 40;
            _lineHeight = 12;
            _isDisplayed = true;
            _isFocused = false;
            _font = new Font("Courier New", 12);

            State.Current = new State(0, 0, 5);

            Width = 12 *_cols;
            Height = _lineHeight * State.Current.MaxLine;

            frame = 0;
        }


        public void Update()
        {
            var current = State.Current;

            if (Input.Space.Pushed) _isFocused = true;

            if (Input.LeftButton.Pushed && Contains(Input.Mouse.Position))
            {
                _isFocused = true;
                int targetLine = (int) (Input.Mouse.Position.Y - this.MinY)/12;
                int targetCursor = (int)(Input.Mouse.Position.X - this.MinX) / 10;
                current.Line = targetLine < current.MaxLine ? targetLine : current.MaxLine;
                current.Cursor = targetCursor < current.Text[current.Line].Length ? targetCursor : current.Text[current.Line].Length;
            }

            if (!_isFocused) return;

            if (Input.Up.Pushed)
            {
                current.Line--;
                if (current.Line < 0) current.Line = 0;
            }
            if (Input.Down.Pushed)
            {
                current.Line++;
                if (current.Line == current.MaxLine) current.Line = current.MaxLine - 1;
                if (current.Text[current.Line].Length < current.Cursor) current.Cursor = current.Text[current.Line].Length;
            }

            if (Input.Right.Pushed) current.Cursor++;
            if (Input.Left.Pushed) current.Cursor--;
            if (current.Cursor < 0)
            {
                if (current.Line == 0) current.Cursor = 0;
                else current.Cursor = current.Text[--current.Line].Length;
            }
            if (current.Cursor > current.Text[current.Line].Length)
            {
                if (current.Line == current.MaxLine - 1)
                {
                    current.Cursor = current.Text[current.Line].Length;
                }
                else
                {
                    current.Line++;
                    current.Cursor = 0;
                }
            }

            if (Input.Enter.Pushed)
            {
                State.Record(current);
                current = State.Current;
                var c = new char[current.Text[current.Line].Length - current.Cursor];
                var str = new StringBuilder();
                current.Text[current.Line].CopyTo(current.Cursor, c, 0, current.Text[current.Line].Length - current.Cursor);
                current.Text[current.Line].Remove(current.Cursor, current.Text[current.Line].Length - current.Cursor);
                str.Insert(0, c);

                current.Cursor = 0;
                current.Text.Insert(++current.Line, str);
                current.MaxLine++;
            }
            if (Input.Back.Pushed)
            {
                State.Record(current);
                current = State.Current;
                if (current.Cursor > 0)
                {
                    current.Text[current.Line].Remove(--current.Cursor, 1);
                }
                else if (current.Line > 0)
                {
                    current.Cursor = current.Text[current.Line - 1].Length;
                    current.Text[current.Line - 1].Insert(current.Text[current.Line - 1].Length, current.Text[current.Line].ToString());
                    current.Text.RemoveAt(current.Line);
                    current.Line--;
                    current.MaxLine--;
                }
            }
            if (Input.Delete.Pushed)
            {
                State.Record(current);
                current = State.Current;
                if (current.Cursor < current.Text[current.Line].Length)
                {
                    current.Text[current.Line].Remove(current.Cursor, 1);
                }
                else if (current.Line < current.MaxLine - 1)
                {
                    current.Text[current.Line].Insert(current.Text[current.Line].Length, current.Text[current.Line + 1].ToString());
                    current.Text.RemoveAt(current.Line + 1);
                    current.MaxLine--;
                }
            }

            if (current.Line < 0) current.Line = 0;
            if (current.Line == current.MaxLine) current.Line = current.MaxLine - 1;
            if (current.Cursor < 0)
            {
                if (current.Line == 0) current.Cursor = 0;
                else current.Cursor = current.Text[--current.Line].Length;
            }
            if (current.Cursor > current.Text[current.Line].Length)
            {
                if (current.Line == current.MaxLine - 1)
                {
                    current.Cursor = current.Text[current.Line].Length;
                }
                else
                {
                    current.Line++;
                    current.Cursor = 0;
                }
            }

            if (Input.Tab.Pushed)
            {
                State.Record(current);
                current = State.Current;
                current.Text[current.Line].Insert(current.Cursor, "  ");
                current.Cursor += 2;
            }

            if (Input.Shift.Pressed)
            {
                if (_selectedBegin == null) _selectedBegin = Tuple.Create(current.Line, current.Cursor);
                if (Input.Up.Pushed || Input.Down.Pushed || Input.Right.Pushed || Input.Left.Pushed)
                {
                    _selectedEnd = Tuple.Create(current.Line, current.Cursor);
                }
            }
            else if (_selectedEnd != null && (current.Line != _selectedEnd.Item1 || current.Cursor != _selectedEnd.Item2))
            {
                _selectedBegin = null;
                _selectedEnd = null;
            }

            if (Input.Control.Pressed)
            {
                if (Input.Sp1.Pushed) State.Undo();
                if (Input.Y.Pushed) State.Redo();
                if (Input.A.Pushed)
                {
                    current.Line = current.MaxLine - 1;
                    current.Cursor = current.Text[current.MaxLine - 1].Length;
                    _selectedBegin = Tuple.Create(0, 0);
                    _selectedEnd = Tuple.Create(current.Line, current.Cursor);
                }
            }

            if (Input.KeyBoard.IsDefined) Insert(Input.KeyBoard.TypedChar);

            Height = _lineHeight * current.MaxLine;

            frame++;
        }

        public string GetString()
        {
            string str = "";
            for (int i = 0; i < State.Current.MaxLine; i++)
            {
                str += State.Current.Text[i] + "\n";
            }
            return str;
        }

        public void Insert(char c)
        {
            if (_isFocused && !Input.Control.Pressed)
            {
                State.Record(State.Current);
                State.Current.Text[State.Current.Line].Insert(State.Current.Cursor++, c);
            }
            Input.KeyBoard.Clear();
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
                            int startX = (int)MinX + _selectedBegin.Item2 * 10 + 2;
                            int startY = (int)MinY + _selectedBegin.Item1 * _lineHeight;
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
                        int startX = (int)MinX + _selectedBegin.Item2 * 10 + 2;
                        int startY = (int)MinY + _selectedBegin.Item1 * _lineHeight;
                        int endX = _selectedEnd.Item2 * 10 + 2;
                        int endY = (int)MinY + _selectedEnd.Item1 * _lineHeight;
                        GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, MaxX - startX, _lineHeight + 5);
                        for (int i = _selectedBegin.Item1 + 1; i < _selectedEnd.Item1; i++)
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
                for (int i = 0; i < State.Current.MaxLine; i++)
                {
                    GraphicsContext.DrawString(State.Current.Text[i].ToString(), _font, Brushes.Black, X, Y + i * _lineHeight);
                }
                if (frame % 120 >= 60)
                {
                    GraphicsContext.DrawLine(Pens.Black, X + 10 * State.Current.Cursor + 2, Y + _lineHeight * State.Current.Line + 2, X + 10 * State.Current.Cursor + 2, Y + _lineHeight * (State.Current.Line + 1) + 2);
                }              
            }
        }


    }
}
