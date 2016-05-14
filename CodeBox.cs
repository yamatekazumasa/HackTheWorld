using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
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

            public int Cursor { get; set; }
            public int MaxLine { get; set; }
            public StringBuilder Text { get; private set; }

            public string[] Lines => Text.ToString().Split('\n');

            public Tuple<int, int> CursorPosition
            {
                get
                {
                    string[] lines = Text.ToString().Split('\n');
                    int line, sum;
                    for (sum = line = 0; sum + lines[line].Length < Current.Cursor; sum += lines[line++].Length + 1) { }
                    return Tuple.Create(line, sum);
                }
            }

            public static Tuple<int, int> Position(int cursor)
            {
                string[] lines = Current.Text.ToString().Split('\n');
                int line, sum;
                for (sum = line = 0; sum + lines[line].Length < cursor; sum += lines[line++].Length + 1) { }
                return Tuple.Create(line, sum);
            }

            public static State Current
            {
                get { return _state[_current]; }
                set { _state[_current] = value; }
            }

            public State(int cursor, int maxLine)
            {
                Cursor = cursor;
                MaxLine = maxLine;
                Text = new StringBuilder();
                for (int i = 0; i < maxLine - 1; i++) Text.Append('\n');
            }

            public static void Record(State s)
            {
                _current = (_current + 1) % _length;
                _origin = _current;
                _state[_current] = new State(s.Cursor, s.MaxLine) {Text = new StringBuilder(s.Text.ToString())};
            }

            public static void Undo()
            {
                if (_current > 0) _current = (_current + _length - 1) % _length;
            }

            public static void Redo()
            {
                if (_state[_current + 1] != null && _current < _origin) _current = (_current + 1) % _length;
            }

            public void ReadFrom(string text)
            {
                Cursor = 0;
                MaxLine = text.Split('\n').Length;
                Text = new StringBuilder(text);
            }

        }

        private int _selectedBegin;
        private int _selectedEnd;
        private int _lineHeight;
        private int _cols;
        private string _clip;
        private bool _isDisplayed;
        private bool _isFocused;
        private readonly Font _font;
        private int frame;

        public bool IsFocused => _isFocused;

        public bool TextSelected => _selectedEnd != -1;

        public CodeBox()
        {
            _cols = 40;
            _lineHeight = 12;
            _isDisplayed = true;
            _isFocused = false;
            _selectedBegin = -1;
            _selectedEnd = -1;
            _font = new Font("Courier New", 12);

            State.Current = new State(0, 5);

            Width = 12 * _cols;
            Height = _lineHeight * State.Current.MaxLine;

            frame = 0;
        }

        public void Update()
        {
            var current = State.Current;
            var lines = current.Lines;
            var pos = current.CursorPosition;
//
//            for (_sum = _line = 0; _sum + lines[_line].Length < current.Cursor; _sum += lines[_line++].Length + 1) { }

            if (Input.Space.Pushed) _isFocused = true;

            if (Input.LeftButton.Pushed && !Contains(Input.Mouse.Position))
            {
                _isFocused = false;
            }

            if (Input.LeftButton.Pressed && Contains(Input.Mouse.Position))
            {
                _isFocused = true;
                int targetLine = (int)(Input.Mouse.Position.Y - this.MinY) / _lineHeight;
                int targetCursor = (int)(Input.Mouse.Position.X - this.MinX) / 10;
//                current.Line = targetLine < current.MaxLine ? targetLine : current.MaxLine;
//                current.Cursor = targetCursor < current.Text[current.Line].Length ? targetCursor : current.Text[current.Line].Length;
            }

            if (!_isFocused) return;

            if (Input.Up.Pushed)
            {
                if (pos.Item1 == 0) current.Cursor = 0;
                else
                {
                    if (current.Cursor - pos.Item2 < lines[pos.Item1 - 1].Length) current.Cursor -= lines[pos.Item1 - 1].Length + 1;
                    else                                                 current.Cursor = pos.Item2 - 1;
                }
            }
            if (Input.Down.Pushed)
            {
                if (current.Cursor + lines[pos.Item1].Length + 1 > current.Text.Length) current.Cursor = current.Text.Length;
                else
                {
                    if (current.Cursor - pos.Item2 < lines[pos.Item1+1].Length) current.Cursor += lines[pos.Item1].Length + 1;
                    else                                               current.Cursor = pos.Item2 + lines[pos.Item1].Length + lines[pos.Item1+1].Length + 1;
                }
            }

            if (Input.Right.Pushed) current.Cursor++;
            if (Input.Left.Pushed) current.Cursor--;
            if (current.Cursor < 0)
            {
                current.Cursor = 0;
            }
            if (current.Cursor > current.Text.Length)
            {
                current.Cursor = current.Text.Length;
            }

            if (Input.Enter.Pushed)
            {
                State.Record(current);
                current = State.Current;
                current.Text.Insert(current.Cursor++, '\n');
                current.MaxLine++;
            }
            if (Input.Back.Pushed && current.Cursor > 0)
            {
                State.Record(current);
                current = State.Current;
                bool isLineFeedCode = current.Text.ToString()[current.Cursor - 1] == '\n';
                current.Text.Remove(--current.Cursor, 1);
                if (isLineFeedCode) current.MaxLine--;
            }
            if (Input.Delete.Pushed && current.Cursor < current.Text.Length)
            {
                State.Record(current);
                current = State.Current;
                bool isLineFeedCode = current.Text.ToString()[current.Cursor] == '\n';
                current.Text.Remove(current.Cursor, 1);
                if (isLineFeedCode) current.MaxLine--;
            }

            if (Input.Tab.Pushed)
            {
                State.Record(current);
                current = State.Current;
                current.Text.Insert(current.Cursor, "  ");
                current.Cursor += 2;
            }

//            if (Input.Shift.Pressed) //シフトキーでの選択
//            {
//                if (_selectedBegin == null) _selectedBegin = Tuple.Create(current.Line, current.Cursor);
//                if (Input.Up.Pushed || Input.Down.Pushed || Input.Right.Pushed || Input.Left.Pushed)
//                {
//                    _selectedEnd = Tuple.Create(current.Line, current.Cursor);
//                }
//            } else if (Input.LeftButton.Pressed && Contains(Input.Mouse.Position)) //マウスでの選択
//            {
//                if (_selectedBegin == null) _selectedBegin = Tuple.Create(current.Line, current.Cursor);
//                if (_selectedBegin.Item1 != current.Line || _selectedBegin.Item2 != current.Cursor)
//                {
//                    _selectedEnd = Tuple.Create(current.Line, current.Cursor);
//                }
//            }
//            else if (_selectedEnd != null && (current.Line != _selectedEnd.Item1 || current.Cursor != _selectedEnd.Item2))
//            {
//                _selectedBegin = null;
//                _selectedEnd = null;
//            }
//            if (Input.LeftButton.Pushed && Contains(Input.Mouse.Position))
//            {
//                _selectedBegin = null;
//                _selectedEnd = null;
//            }
//
            if (Input.Control.Pressed)
            {
                if (Input.Z.Pushed) State.Undo();
                if (Input.Y.Pushed) State.Redo();
                if (Input.A.Pushed)
                {
                    current.Cursor = current.Text.Length;
                    _selectedBegin = 0;
                    _selectedEnd = current.Text.Length;
                }
//                if (Input.R.Pushed)
//                {
//                    StreamReader sr = new StreamReader(@".\code.json", Encoding.GetEncoding("utf-8"));
//                    CodeData o = JsonConvert.DeserializeObject<CodeData>(sr.ReadToEnd());
//                    State.Current.ReadFrom(o.text);
//                    sr.Close();
//                }
//                if (Input.S.Pushed)
//                {
//                    CodeData obj = new CodeData { type = "Block", text = GetString(), date = DateTime.Now.ToString() };
//                    string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
//                    StreamWriter sw = new StreamWriter(@".\code.json", false, Encoding.GetEncoding("utf-8"));
//                    sw.Write(json);
//                    sw.Close();
//                }
//                if (Input.C.Pushed)
//                {
//                    if (_selectedEnd != null)
//                    {
//                        WindowContext.Invoke((Action)(() => {
//                            string str;
//                            if (_selectedBegin.Item1 == _selectedEnd.Item1)
//                            {
//                                str = current.Text[selectedBegin.Item1].ToString().Substring(selectedBegin.Item2, SelectedEnd.Item2 - selectedBegin.Item2);
//                            }
//                            else
//                            {
//                                str = current.Text[selectedBegin.Item1].ToString().Substring(selectedBegin.Item2, current.Text[selectedBegin.Item1].Length - selectedBegin.Item2) + "\n";
//                                for (int i = selectedBegin.Item1 + 1; i < SelectedEnd.Item1; i++)
//                                {
//                                    str += current.Text[i] + "\n";
//                                }
//                                str += current.Text[SelectedEnd.Item1].ToString().Substring(0, SelectedEnd.Item2);
//                            }
//                            Clipboard.SetDataObject(str);
//                        }));
//                    }
//                }
//                if (Input.X.Pushed)
//                {
//                    if (_selectedEnd != null)
//                    {
//                        State.Record(current);
//                        current = State.Current;
//                        WindowContext.Invoke((Action)(() => {
//                            string str;
//                            if (_selectedBegin.Item1 == _selectedEnd.Item1)
//                            {
//                                str = current.Text[selectedBegin.Item1].ToString().Substring(selectedBegin.Item2, SelectedEnd.Item2 - selectedBegin.Item2);
//                                current.Text[selectedBegin.Item1].Remove(selectedBegin.Item2, SelectedEnd.Item2 - selectedBegin.Item2);
//                            }
//                            else
//                            {
//                                str = current.Text[selectedBegin.Item1].ToString().Substring(selectedBegin.Item2, current.Text[selectedBegin.Item1].Length - selectedBegin.Item2) + "\n";
//                                for (int i = selectedBegin.Item1 + 1; i < SelectedEnd.Item1; i++)
//                                {
//                                    str += current.Text[i] + "\n";
//                                }
//                                str += current.Text[SelectedEnd.Item1].ToString().Substring(0, SelectedEnd.Item2);
//                                current.Text[selectedBegin.Item1].Remove(selectedBegin.Item2, current.Text[selectedBegin.Item1].Length - selectedBegin.Item2);
//                                current.Text.RemoveRange(selectedBegin.Item1 + 1, SelectedEnd.Item1 - 1);
//                                current.Text[selectedBegin.Item1 + 1].Remove(0, SelectedEnd.Item2);
//                                current.MaxLine -= SelectedEnd.Item1 - selectedBegin.Item1;
//                                current.Line = selectedBegin.Item1;
//                                current.Cursor = selectedBegin.Item2;
//                            }
//                            Clipboard.SetDataObject(str);
//                        }));
//                    }
//                }
//                if (Input.V.Pushed)
//                {
//                    State.Record(current);
//                    current = State.Current;
//                    WindowContext.Invoke((Action)(() => {
//                        var str = Clipboard.GetText().Split('\n');
//                        current.Text[current.Line].Insert(current.Cursor, str[0]);
//                        for (int i = 1; i < str.Length; i++)
//                        {
//                            current.Text.Insert(current.Line + i, new StringBuilder(str[i]));
//                        }
//                        current.MaxLine += str.Length - 1;
//                        current.Line += str.Length - 1;
//                        current.Cursor = str[str.Length - 1].Length;
//                    }));
//                }

            }

            if (Input.KeyBoard.IsDefined) Insert(Input.KeyBoard.TypedChar);

            Height = _lineHeight * current.MaxLine;

            frame++;
        }

        public string GetString()
        {
            return State.Current.Text.ToString();
        }

        public void Insert(char c)
        {
            if (_isFocused && !Input.Control.Pressed)
            {
                State.Record(State.Current);
                State.Current.Text.Insert(State.Current.Cursor++, c);
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

                string[] lines = State.Current.Text.ToString().Split('\n');
                var pos = State.Current.CursorPosition;

                // 選択範囲の描画
                if (TextSelected)
                {
                    Tuple<int, int> selectedBegin;
                    Tuple<int, int> selectedEnd;
                    if (_selectedBegin < _selectedEnd)
                    {
                        selectedBegin = State.Position(_selectedBegin);
                        selectedEnd = State.Position(_selectedEnd);
                    }
                    else
                    {
                        selectedBegin = State.Position(_selectedEnd);
                        selectedEnd = State.Position(_selectedBegin);
                    }

                    int startX = (int)MinX + selectedBegin.Item2 * 10 + 2;
                    int startY = (int)MinY + selectedBegin.Item1 * _lineHeight;
                    int endX = selectedEnd.Item2 * 10 + 2;
                    int endY = (int)MinY + selectedEnd.Item1 * _lineHeight;

                    if (selectedBegin.Item1 == selectedEnd.Item1)
                    {
                        GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, (selectedEnd.Item2 - selectedBegin.Item2) * 10, _lineHeight + 5);
                    }
                    else
                    {
                        GraphicsContext.FillRectangle(Brushes.LightBlue, startX, startY, MaxX - startX, _lineHeight + 5);
                        for (int i = selectedBegin.Item1 + 1; i < selectedEnd.Item1; i++)
                            GraphicsContext.FillRectangle(Brushes.LightBlue, MinX, MinY + i * _lineHeight, Width, _lineHeight + 5);
                        GraphicsContext.FillRectangle(Brushes.LightBlue, MinX, endY, endX, _lineHeight + 5);
                    }
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    GraphicsContext.DrawString(lines[i], _font, Brushes.Black, X, Y + i * _lineHeight);
                }
                if (frame % 120 > 60)
                {
                    GraphicsContext.DrawLine(Pens.Black, X + 10 * (State.Current.Cursor - pos.Item2) + 2, Y + _lineHeight * pos.Item1 + 2, X + 10 * (State.Current.Cursor - pos.Item2) + 2, Y + _lineHeight * (pos.Item1 + 1) + 2);
                }
                GraphicsContext.DrawString("line: " + pos.Item1 + ", cursor: " + (State.Current.Cursor - pos.Item2) + ", maxline: " + State.Current.MaxLine, _font, Brushes.Black, X, MaxY + 10);
            }
        }


    }
}
