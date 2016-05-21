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
    /// <summary>
    /// IEditable なオブジェクトに付随して作られる。
    /// オブジェクトのスクリプトを編集するテキストエディタ。
    /// </summary>
    public class CodeBox : GameObject
    {
        private int _selectedBegin;
        private int _selectedEnd;
        private readonly int _lineHeight;
        private int _cols;
        private bool _isFocused;
        private readonly Font _font;
        private int _frame;
        private readonly GameObject _subject;
        private readonly CodeState[] _history;
        private int _origin;
        private int _current;
        private readonly int _historyLength;

        public bool IsFocused => _isFocused;
        public void Focus() { _isFocused = true; }

        public bool TextSelected => _selectedEnd != -1;
        public CodeState Current => _history[_current];

        public CodeBox(GameObject obj)
        {
            _cols = 40;
            _lineHeight = 12;
            _isFocused = true;
            _selectedBegin = -1;
            _selectedEnd = -1;
            _subject = obj;
            _historyLength = 50;
            _history = new CodeState[_historyLength];
            _font = new Font("Courier New", 12);

            _history[_current] = new CodeState(0, 5, _subject.ObjectType);

            Width = 12 * _cols;
            Height = _lineHeight * _history[_current].MaxLine;

            _frame = 0;
        }

        public void Update()
        {
            var current = _history[_current];
            var lines = current.Lines;
            var pos = current.CursorPosition;

            if (Input.Space.Pushed) _isFocused = true;

            if (Input.Mouse.Left.Pushed && !Contains(Input.Mouse.Position) && !_subject.Contains(Input.Mouse.Position))
            {
                _isFocused = false;
            }

            if (Input.Mouse.Left.Pressed && Contains(Input.Mouse.Position))
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
                Record(current);
                current = _history[_current];
                current.Text.Insert(current.Cursor++, '\n');
                current.MaxLine++;
            }
            if (Input.Back.Pushed && current.Cursor > 0)
            {
                Record(current);
                current = _history[_current];
                if (_selectedEnd != -1)
                {
                    if (_selectedBegin < _selectedEnd)
                    {
                        current.Cursor = _selectedBegin;
                        current.Text.Remove(_selectedBegin, _selectedEnd - _selectedBegin);
                    }
                    else
                    {
                        current.Cursor = _selectedEnd;
                        current.Text.Remove(_selectedEnd, _selectedBegin - _selectedEnd);
                    }
                }
                else current.Text.Remove(--current.Cursor, 1);
                current.MaxLine = current.Lines.Length;
            }
            if (Input.Delete.Pushed && current.Cursor < current.Text.Length)
            {
                Record(current);
                current = _history[_current];
                if (_selectedEnd != -1)
                {
                    if (_selectedBegin < _selectedEnd)
                    {
                        current.Cursor = _selectedBegin;
                        current.Text.Remove(_selectedBegin, _selectedEnd - _selectedBegin);
                    }
                    else
                    {
                        current.Cursor = _selectedEnd;
                        current.Text.Remove(_selectedEnd, _selectedBegin - _selectedEnd);
                    }
                }
                else current.Text.Remove(current.Cursor, 1);
                current.MaxLine = current.Lines.Length;
            }
            if (Input.Tab.Pushed)
            {
                Record(current);
                current = _history[_current];
                current.Text.Insert(current.Cursor, "  ");
                current.Cursor += 2;
            }

            // 選択範囲の設定
            if (Input.Shift.Pressed || (Input.Mouse.Left.Pressed && Contains(Input.Mouse.Position)))
            {
                if (_selectedBegin == -1) _selectedBegin = current.Cursor;
                if(!Input.Mouse.Left.Pushed) _selectedEnd = current.Cursor;
            }
            if (current.Cursor != _selectedEnd)
            {
                _selectedBegin = -1;
                _selectedEnd = -1;
            }

            if (Input.Control.Pressed)
            {
                if (Input.Z.Pushed) Undo();
                if (Input.Y.Pushed) Redo();
                if (Input.A.Pushed)
                {
                    current.Cursor = current.Text.Length;
                    _selectedBegin = 0;
                    _selectedEnd = current.Text.Length;
                }
                if (Input.R.Pushed)
                {
//                    StreamReader sr = new StreamReader(@".\code.json", Encoding.GetEncoding("utf-8"));
//                    _history[_current] = JsonConvert.DeserializeObject<CodeState>(sr.ReadToEnd());
//                    sr.Close();
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
                        Record(current);
                        current = _history[_current];
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
                            current.MaxLine -= _history[_current].Position(_selectedEnd).Item1 - _history[_current].Position(_selectedEnd).Item1;
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
                    Record(current);
                    current = _history[_current];
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

            _frame++;
        }

        /// <summary>
        /// 現在表示されているコードを取得する。
        /// </summary>
        public string GetString()
        {
            return _history[_current].Text.ToString();
        }

        /// <summary>
        /// 現在のカーソルの位置に、文字を挿入する。
        /// </summary>
        public void Insert(char c)
        {
            if (_isFocused && !Input.Control.Pressed)
            {
                Record(_history[_current]);
                _history[_current].Text.Insert(_history[_current].Cursor++, c);
            }
            Input.KeyBoard.Clear();
        }

        /// <summary>
        /// 現在のコードの状態を保存して、履歴を次に進める。
        /// </summary>
        public void Record(CodeState s)
        {
            var name = _history[_current].Name;
            _current = (_current + 1) % _historyLength;
            _origin = _current;
            _history[_current] = new CodeState(s.Cursor, s.MaxLine, name) {
                Text = new StringBuilder(s.Text.ToString()),
                UpdatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// 操作を一つ戻す。
        /// </summary>
        public void Undo()
        {
            if (_current > 0) _current = (_current + _historyLength - 1) % _historyLength;
        }

        /// <summary>
        /// 戻した操作を一つやりなおす。
        /// </summary>
        public void Redo()
        {
            if (_history[_current + 1] != null && _current < _origin) _current = (_current + 1) % _historyLength;
        }

        /// <summary>
        /// 現在のシーンが EditScene で、
        /// 自身または紐つけられたオブジェクトがフォーカスされているとき、描画する。
        /// </summary>
        public override void Draw()
        {
            if (_isFocused && Scene.Current is EditScene)
            {
                // 編集部分の描画
                GraphicsContext.FillRectangle(Brushes.Azure, this);
                GraphicsContext.DrawRectangle(Pens.ForestGreen, this);

                // オブジェクトの名前の描画
                GraphicsContext.FillRectangle(Brushes.LightGreen, X, Y - 20, W, 20);
                GraphicsContext.DrawRectangle(Pens.ForestGreen, X, Y - 20, W, 20);
                GraphicsContext.DrawString(_history[_current].Name.ToString(), _font, Brushes.Black, X, Y - 20);

                string[] lines = _history[_current].Lines;
                var pos = _history[_current].CursorPosition;

                // 選択範囲の描画
                if (TextSelected)
                {
                    Tuple<int, int> selectedBegin;
                    Tuple<int, int> selectedEnd;
                    if (_selectedBegin < _selectedEnd)
                    {
                        selectedBegin = _history[_current].Position(_selectedBegin);
                        selectedEnd = _history[_current].Position(_selectedEnd);
                    }
                    else
                    {
                        selectedBegin = _history[_current].Position(_selectedEnd);
                        selectedEnd = _history[_current].Position(_selectedBegin);
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
                // 文字の描画
                for (int i = 0; i < lines.Length; i++)
                {
                    GraphicsContext.DrawString(lines[i], _font, Brushes.Black, X, Y + i * _lineHeight);
                }
                // カーソルの描画
                if (_frame % 120 > 60)
                {
                    GraphicsContext.DrawLine(Pens.Black, X + 10 * pos.Item2 + 2, Y + _lineHeight * pos.Item1 + 2, X + 10 * pos.Item2 + 2, Y + _lineHeight * (pos.Item1 + 1) + 2);
                }
                // デバッグ用の文字列の描画
                GraphicsContext.DrawString("line: " + pos.Item1 + ", cursor: " + pos.Item2 + ", maxline: " + _history[_current].MaxLine, _font, Brushes.Black, X, MaxY + 10);
            }
        }


    }
}
