using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
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
        private IEditable _focusingObject;
        private readonly Font _font;
        private readonly Pen _pen;
        private int _frame;
        private CodeState[] _history;
        private int _origin;
        private int _current;
        private readonly int _historyLength;

        public bool TextSelected => _selectedEnd != -1;
        public CodeState Current => _history[_current];

        public CodeBox(IEditable obj)
        {
            _cols = 40;
            _lineHeight = 12;
            _selectedBegin = -1;
            _selectedEnd = -1;
            _focusingObject = obj;
            _historyLength = 50;
            _history = new CodeState[_historyLength];
            _font = new Font("Courier New", 12);
            _pen = new Pen(Color.Black, 30);

            _history[_current] = new CodeState(0, 5);

            X = CellSize*CellNumX;
            Width = 12 * _cols;
            Height = 600;

            _frame = 0;
        }

        public void Update()
        {
            var current = _history[_current];
            var lines = current.Lines;
            var pos = current.CursorPosition;

            if (Input.Mouse.Left.Pressed && Contains(Input.Mouse.Position))
            {
                int line = (int)(Input.Mouse.Y - MinY) / _lineHeight;
                int targetLine = line < current.MaxLine ? line : current.MaxLine - 1;
                int targetCursor = (int)(Input.Mouse.X - MinX) / 10;
                current.Cursor = targetCursor < lines[targetLine].Length ? targetCursor : lines[targetLine].Length;
                for (int i = 0; i < targetLine; i++)
                {
                    current.Cursor += lines[i].Length + 1;
                }
            }

            if ((Input.Left.Pushed || Input.Left.Pressed && Counter() > 50 && Counter() % 10 == 0) && current.Cursor > 0) current.Cursor--;
            if ((Input.Right.Pushed || Input.Right.Pressed && Counter() > 50 && Counter() % 10 == 0) && current.Cursor < current.Text.Length) current.Cursor++;
            if (Input.Up.Pushed || Input.Up.Pressed && Counter() > 50 && Counter() % 10 == 0)
            {
                if (pos.Item1 == 0) current.Cursor = 0;
                else
                {
                    if (pos.Item2 <= lines[pos.Item1 - 1].Length) current.Cursor -= lines[pos.Item1 - 1].Length + 1;
                    else                                          current.Cursor -= pos.Item2 + 1;
                }
            }
            if (Input.Down.Pushed || Input.Down.Pressed && Counter() > 50 && Counter() % 10 == 0)
            {
                if (pos.Item1 == current.MaxLine - 1) current.Cursor = current.Text.Length;
                else
                {
                    if (pos.Item2 <= lines[pos.Item1+1].Length) current.Cursor += lines[pos.Item1].Length + 1;
                    else                                        current.Cursor += lines[pos.Item1].Length + lines[pos.Item1 + 1].Length - pos.Item2 + 1;
                }
            }

            if (Input.Enter.Pushed || Input.Enter.Pressed && Counter() > 50 && Counter() % 10 == 0)
            {
                Record(current);
                current = _history[_current];
                current.Text.Insert(current.Cursor++, '\n');
                current.MaxLine++;
            }
            if ((Input.Back.Pushed || Input.Back.Pressed && Counter() > 50 && Counter() % 10 == 0) && current.Cursor > 0)
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
            if ((Input.Delete.Pushed || Input.Delete.Pressed && Counter() > 50 && Counter() % 10 == 0) && current.Cursor < current.Text.Length)
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

            if (Input.Left.Released || Input.Right.Released || Input.Up.Released || Input.Down.Released || Input.Delete.Released || Input.Back.Released || Input.Enter.Released)
            {
                Counter = CreateCounter();
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

            _focusingObject.Code = _history[_current].Text.ToString();

            // ターゲティングのアニメーション
            if (_frame%200 >= 100) _pen.Width++;
            else _pen.Width--;

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
            if (!Input.Control.Pressed)
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
            _current = (_current + 1) % _historyLength;
            _origin = _current;
            _history[_current] = new CodeState(s.Cursor, s.MaxLine) {
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
        /// 引数に与えられたオブジェクトにフォーカスする。
        /// </summary>
        /// <param name="obj"></param>
        public void Focus(IEditable obj)
        {
            _focusingObject = obj;
            _history = new CodeState[_historyLength];
            _current = 0;
            _origin = 0;
            _history[_current] = new CodeState(0, obj.Code.Split('\n').Length)
            {
                Text = new StringBuilder(obj.Code),
                UpdatedAt = DateTime.Now
            };

        }

        /// <summary>
        /// 現在のシーンが EditScene で、
        /// 自身または紐つけられたオブジェクトがフォーカスされているとき、描画する。
        /// </summary>
        public override void Draw()
        {
            // 編集部分の描画
            GraphicsContext.FillRectangle(Brushes.Azure, this);
            GraphicsContext.DrawRectangle(Pens.ForestGreen, this);

            // オブジェクトの名前の描画
            GraphicsContext.FillRectangle(Brushes.LightGreen, X, Y, W, 20);
            GraphicsContext.DrawRectangle(Pens.ForestGreen, X, Y, W, 20);
            GraphicsContext.DrawString(_focusingObject.Name, _font, Brushes.Black, X, Y);

            // ターゲティングの描画
            GraphicsContext.DrawEllipse(_pen, _focusingObject.X, _focusingObject.Y, _focusingObject.Width, _focusingObject.Height);
            GraphicsContext.DrawLine(Pens.Black, _focusingObject.Position + new Vector(30, 30), Position + new Vector(0, 10));

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
                int beginY = (int)MinY + selectedBegin.Item1 * _lineHeight + 20;
                int endX = selectedEnd.Item2 * 10 + 2;
                int endY = (int)MinY + selectedEnd.Item1 * _lineHeight + 20;

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
                GraphicsContext.DrawString(lines[i], _font, Brushes.Black, X, Y + i * _lineHeight + 20);
            }
            // カーソルの描画
            if (_frame % 60 > 20)
            {
                GraphicsContext.DrawLine(Pens.Black, X + 10 * pos.Item2 + 2, Y + _lineHeight * pos.Item1 + 22, X + 10 * pos.Item2 + 2, Y + _lineHeight * (pos.Item1 + 1) + 22);
            }
            // デバッグ用の文字列の描画
            GraphicsContext.DrawString("line: " + pos.Item1 + ", cursor: " + pos.Item2 + ", maxline: " + _history[_current].MaxLine, _font, Brushes.Black, X, MaxY + 10);
        }

    }
}
