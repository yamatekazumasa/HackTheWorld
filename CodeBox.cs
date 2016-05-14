using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class CodeBox : GameObject
    {
        [JsonObject(MemberSerialization.OptIn)]
        class State
        {
            private static State[] _state = new State[50];
            private static int _origin;
            private static int _current;
            private static readonly int _length = 50;

            [JsonProperty("cursor", Order = 0)]
            public int Cursor { get; set; }
            [JsonProperty("maxline", Order = 1)]
            public int MaxLine { get; set; }
            [JsonProperty("name", Order = 2)]
            [JsonConverter(typeof(StringEnumConverter))]
            public ObjectType Name { get; set; }
            [JsonProperty("text", Order = 3)]
            public StringBuilder Text { get; private set; }
            [JsonProperty("updatedAt", Order = 4)]
            public DateTime UpdatedAt { get; set; }

            public string[] Lines => Text.ToString().Split('\n');

            public Tuple<int, int> CursorPosition
            {
                get
                {
                    string[] lines = Text.ToString().Split('\n');
                    int line, sum;
                    for (sum = line = 0; sum + lines[line].Length < Current.Cursor; sum += lines[line++].Length + 1) { }
                    return Tuple.Create(line, Cursor - sum);
                }
            }

            public static Tuple<int, int> Position(int cursor)
            {
                string[] lines = Current.Text.ToString().Split('\n');
                int line, sum;
                for (sum = line = 0; sum + lines[line].Length < cursor; sum += lines[line++].Length + 1) { }
                return Tuple.Create(line, cursor - sum);
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
                Name = ObjectType.Block;
                Text = new StringBuilder();
                for (int i = 0; i < maxLine - 1; i++) Text.Append('\n');
            }

            public static void Record(State s)
            {
                _current = (_current + 1) % _length;
                _origin = _current;
                _state[_current] = new State(s.Cursor, s.MaxLine) {Text = new StringBuilder(s.Text.ToString())};
                _state[_current].UpdatedAt = DateTime.Now;
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
        private bool _isFocused;
        private readonly Font _font;
        private int frame;
        private GameObject _subject;

        public bool IsFocused => _isFocused;
        public void Focus() { _isFocused = true; }

        public bool TextSelected => _selectedEnd != -1;

        public CodeBox(GameObject obj)
        {
            _cols = 40;
            _lineHeight = 12;
            _isFocused = false;
            _selectedBegin = -1;
            _selectedEnd = -1;
            _subject = obj;
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

            if (Input.Space.Pushed) _isFocused = true;

            if (Input.LeftButton.Pushed && !Contains(Input.Mouse.Position) && !_subject.Contains(Input.Mouse.Position))
            {
                _isFocused = false;
            }

            if (Input.LeftButton.Pressed && Contains(Input.Mouse.Position))
            {
                _isFocused = true;
                int l = (int)(Input.Mouse.Y - MinY) / _lineHeight;
                int targetLine = l < current.MaxLine ? l : current.MaxLine;
                int targetCursor = (int)(Input.Mouse.X - MinX) / 10;
                current.Cursor = targetCursor < lines[targetLine].Length ? targetCursor : lines[targetLine].Length;
                for (int i = 0; i < targetLine; i++)
                {
                    current.Cursor += lines[i].Length + 1;
                }
            }

            if (!_isFocused) return;

            if (Input.Left.Pushed && current.Cursor > 0) current.Cursor--;
            if (Input.Right.Pushed && current.Cursor < current.Text.Length) current.Cursor++;
            if (Input.Up.Pushed)
            {
                if (pos.Item1 == 0) current.Cursor = 0;
                else
                {
                    if (pos.Item2 <= lines[pos.Item1 - 1].Length) current.Cursor -= lines[pos.Item1 - 1].Length + 1;
                    else                                          current.Cursor -= pos.Item2 + 1;
                }
            }
            if (Input.Down.Pushed)
            {
                if (pos.Item1 == current.MaxLine - 1) current.Cursor = current.Text.Length;
                else
                {
                    if (pos.Item2 <= lines[pos.Item1+1].Length) current.Cursor += lines[pos.Item1].Length + 1;
                    else                                        current.Cursor += lines[pos.Item1].Length + lines[pos.Item1 + 1].Length - pos.Item2 + 1;
                }
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

            // 選択範囲の設定
            if (Input.Shift.Pressed || (Input.LeftButton.Pressed && Contains(Input.Mouse.Position)))
            {
                if (_selectedBegin == -1) _selectedBegin = current.Cursor;
                if(!Input.LeftButton.Pushed) _selectedEnd = current.Cursor;
            }
            if (current.Cursor != _selectedEnd)
            {
                _selectedBegin = -1;
                _selectedEnd = -1;
            }

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
                if (Input.R.Pushed)
                {
                    StreamReader sr = new StreamReader(@".\code.json", Encoding.GetEncoding("utf-8"));
                    State.Current = JsonConvert.DeserializeObject<State>(sr.ReadToEnd());
                    sr.Close();
                }
                if (Input.S.Pushed)
                {
                    string json = JsonConvert.SerializeObject(current, Formatting.Indented);
                    StreamWriter sw = new StreamWriter(@".\code.json", false, Encoding.GetEncoding("utf-8"));
                    sw.Write(json);
                    sw.Close();
                }
                if (Input.C.Pushed)
                {
                    if (_selectedEnd != -1)
                    {
                        WindowContext.Invoke((Action)(() => {
                            if (_selectedBegin > _selectedEnd)
                            {
                                int tmp = _selectedBegin;
                                _selectedBegin = _selectedEnd;
                                _selectedEnd = tmp;
                            }
                            int length = _selectedEnd - _selectedBegin;
                            char[] c = new char[length];
                            current.Text.CopyTo(_selectedBegin, c, 0, length);
                            Clipboard.SetDataObject(new string(c));
                        }));
                    }
                }
                if (Input.X.Pushed)
                {
                    if (_selectedEnd != -1)
                    {
                        State.Record(current);
                        current = State.Current;
                        WindowContext.Invoke((Action)(() => {
                            if (_selectedBegin > _selectedEnd)
                            {
                                int tmp = _selectedBegin;
                                _selectedBegin = _selectedEnd;
                                _selectedEnd = tmp;
                            }
                            int length = _selectedEnd - _selectedBegin;
                            char[] c = new char[length];
                            current.Cursor = _selectedBegin;
                            current.MaxLine -= State.Position(_selectedEnd).Item1 - State.Position(_selectedEnd).Item1;
                            current.Text.CopyTo(_selectedBegin, c, 0, length);
                            current.Text.Remove(_selectedBegin, length);
                            _selectedBegin = -1;
                            _selectedEnd = -1;
                            Clipboard.SetDataObject(new string(c));
                        }));
                    }
                }
                if (Input.V.Pushed)
                {
                    State.Record(current);
                    current = State.Current;
                    WindowContext.Invoke((Action)(() => {
                        var str = Clipboard.GetText();
                        current.Text.Insert(current.Cursor, str);
                        current.MaxLine = current.Text.ToString().Split('\n').Length;
                        current.Cursor += str.Length;
                    }));
                }

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
            if (_isFocused)
            {
                if (_isFocused) GraphicsContext.FillRectangle(Brushes.Azure, this);
                else GraphicsContext.FillRectangle(Brushes.DarkSeaGreen, this);
                GraphicsContext.DrawRectangle(Pens.ForestGreen, this);
                GraphicsContext.FillRectangle(Brushes.LightGreen, X, Y - 20, W, 20);
                GraphicsContext.DrawRectangle(Pens.ForestGreen, X, Y - 20, W, 20);
                GraphicsContext.DrawString(State.Current.Name.ToString(), _font, Brushes.Black, X, Y - 20);

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

                    int beginX = (int)MinX + selectedBegin.Item2 * 10 + 2;
                    int beginY = (int)MinY + selectedBegin.Item1 * _lineHeight;
                    int endX = selectedEnd.Item2 * 10 + 2;
                    int endY = (int)MinY + selectedEnd.Item1 * _lineHeight;

                    if (selectedBegin.Item1 == selectedEnd.Item1)
                    {
                        GraphicsContext.FillRectangle(Brushes.LightBlue, beginX, beginY, (selectedEnd.Item2 - selectedBegin.Item2) * 10, _lineHeight + 5);
                    }
                    else
                    {
                        GraphicsContext.FillRectangle(Brushes.LightBlue, beginX, beginY, MaxX - beginX, _lineHeight + 5);
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
                    GraphicsContext.DrawLine(Pens.Black, X + 10 * pos.Item2 + 2, Y + _lineHeight * pos.Item1 + 2, X + 10 * pos.Item2 + 2, Y + _lineHeight * (pos.Item1 + 1) + 2);
                }
                GraphicsContext.DrawString("line: " + pos.Item1 + ", cursor: " + pos.Item2 + ", maxline: " + State.Current.MaxLine, _font, Brushes.Black, X, MaxY + 10);
            }
        }


    }
}
