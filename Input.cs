using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HackTheWorld
{
    /// <summary>
    /// 入力を静的に取得するクラス。
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// キー入力を取得する。
        /// </summary>
        public class Key
        {
            /// <summary>
            /// 一つ前のフレームでキーが押されていたか否か。
            /// </summary>
            private bool _wasPressed;
            /// <summary>
            /// 現在キーが押されているか否か。
            /// </summary>
            private bool _isPressed;

            /// <summary>
            /// 現在キーが押下されているかどうかを見て、自身の状態を更新する。
            /// </summary>
            /// <param name="isPressed">現在キーが押されているか否か。</param>
            public void Append(bool isPressed)
            {
                _wasPressed = _isPressed;
                _isPressed = isPressed;
            }

            /// <summary>
            /// 前後の状態は関係なく、現在キーが押されていたら true を返す。
            /// </summary>
            public bool Pressed => _isPressed;
            /// <summary>
            /// 一つ前のフレームでキーが押されておらず、現在のフレームで初めてキーが押された場合に true を返す。
            /// </summary>
            public bool Pushed => !_wasPressed && _isPressed;
            /// <summary>
            /// 一つ前のフレームでキーが押されており、現在のフレームでキーが離された場合に true を返す。
            /// </summary>
            public bool Released => _wasPressed && !_isPressed;
        }

        /// <summary>
        /// マウスの座標とボタンの状態を取得する。
        /// </summary>
        public static class Mouse
        {
            /// <summary>
            /// 現在のマウスのウィンドウに対する相対座標。
            /// </summary>
            private static Vector _position;
            /// <summary>
            /// 一つ前のフレームでのマウスのウィンドウに対する相対座標。
            /// </summary>
            private static Vector _previousPosition;
            /// <summary>
            /// マウスの左ボタン。
            /// </summary>
            public static MouseButton Left { get; } = new MouseButton();
            /// <summary>
            /// マウスの右ボタン。
            /// </summary>
            public static MouseButton Right { get; } = new MouseButton();

            /// <summary>
            /// マウスのボタン入力を取得する。
            /// </summary>
            public class MouseButton
            {
                /// <summary>
                /// 一つ前のフレームでボタンが押されていたか否か。
                /// </summary>
                private bool _wasPressed;
                /// <summary>
                /// 現在ボタンが押されているか否か。
                /// </summary>
                private bool _isPressed;

                /// <summary>
                /// 現在マウスボタンが押下されているかどうかを見て、自身の状態を更新する。
                /// </summary>
                /// <param name="isPressed">現在ボタンが押されているか否か。</param>
                public void Append(bool isPressed)
                {
                    _wasPressed = _isPressed;
                    _isPressed = isPressed;
                }

                /// <summary>
                /// 前後の状態は関係なく、現在ボタンが押されていたら true を返す。
                /// </summary>
                public bool Pressed => _isPressed;
                /// <summary>
                /// 一つ前のフレームでボタンが押されておらず、現在のフレームで初めてボタンが押された場合に true を返す。
                /// </summary>
                public bool Pushed => !_wasPressed && _isPressed;
                /// <summary>
                /// 一つ前のフレームでボタンが押されており、現在のフレームでボタンが離された場合に true を返す。
                /// </summary>
                public bool Released => _wasPressed && !_isPressed;
            }

            /// <summary>
            /// マウスポインタの座標の更新。
            /// </summary>
            public static void Update(Point mousePosition, Point windowPosition)
            {
                _previousPosition = _position;
                _position = mousePosition.ToVector() - windowPosition.ToVector() - new Vector(9, 30);
            }

            /// <summary>
            /// 押されたボタンの状態を更新する。
            /// </summary>
            public static void ButtonAppend(MouseButtons b)
            {
                if (b == MouseButtons.Left) Left.Append(true);
                if (b == MouseButtons.Right) Right.Append(true);
            }
            /// <summary>
            /// 離されたボタンの状態を更新する。
            /// </summary>
            public static void ButtonDisappend(MouseButtons b)
            {
                if (b == MouseButtons.Left) Left.Append(false);
                if (b == MouseButtons.Right) Right.Append(false);
            }

            /// <summary>
            /// マウスポインタの X 座標。
            /// </summary>
            public static int X => (int)_position.X;
            /// <summary>
            /// マウスポインタの Y 座標。
            /// </summary>
            public static int Y => (int)_position.Y;
            /// <summary>
            /// マウスポインタの座標。
            /// </summary>
            public static Vector Position => _position;
            /// <summary>
            /// マウスポインタが動かされたか否か。
            /// </summary>
            public static bool Moved => _position != _previousPosition;
        }

        /// <summary>
        /// キーボード入力を取得する。
        /// </summary>
        public static class KeyBoard
        {
            /// <summary>
            /// 押された文字。
            /// キーボードのキーではなく、実際に入力された文字が入る。
            /// 例えば Shift+'a' が押された場合は 'A' が入る。
            /// </summary>
            static char _buffer;

            /// <summary>
            /// 状態の更新。
            /// </summary>
            public static void Append(char c)
            {
                _buffer = c;
            }

            /// <summary>
            /// null 値で更新（clear）。
            /// </summary>
            public static void Clear()
            {
                _buffer = '\0';
            }

            /// <summary>
            /// 押された文字があるか。
            /// </summary>
            public static bool IsDefined => _buffer != '\0';

            /// <summary>
            /// 押された文字。
            /// </summary>
            public static char TypedChar => _buffer;

        }

        public static void Update(LinkedList<Keys> pressedKeys)
        {
            Up.Append(pressedKeys.Contains(Keys.Up));
            Down.Append(pressedKeys.Contains(Keys.Down));
            Left.Append(pressedKeys.Contains(Keys.Left));
            Right.Append(pressedKeys.Contains(Keys.Right));
            Z.Append(pressedKeys.Contains(Keys.Z));
            X.Append(pressedKeys.Contains(Keys.X));
            V.Append(pressedKeys.Contains(Keys.V));
            R.Append(pressedKeys.Contains(Keys.R));
            C.Append(pressedKeys.Contains(Keys.C));
            A.Append(pressedKeys.Contains(Keys.A));
            W.Append(pressedKeys.Contains(Keys.W));
            S.Append(pressedKeys.Contains(Keys.S));
            Y.Append(pressedKeys.Contains(Keys.Y));
            Enter.Append(pressedKeys.Contains(Keys.Enter));
            Space.Append(pressedKeys.Contains(Keys.Space));
            Tab.Append(pressedKeys.Contains(Keys.Tab));
            Shift.Append(pressedKeys.Contains(Keys.ShiftKey));
            Control.Append(pressedKeys.Contains(Keys.ControlKey));
            Back.Append(pressedKeys.Contains(Keys.Back));
            Delete.Append(pressedKeys.Contains(Keys.Delete));
        }

        public static Key Up { get; } = new Key();
        public static Key Down { get; } = new Key();
        public static Key Left { get; } = new Key();
        public static Key Right { get; } = new Key();
        public static Key Z { get; } = new Key();
        public static Key X { get; } = new Key();
        public static Key C { get; } = new Key();
        public static Key V { get; } = new Key();
        public static Key R { get; } = new Key();
        public static Key A { get; } = new Key();
        public static Key S { get; } = new Key();
        public static Key Y { get; } = new Key();
        public static Key W { get; } = new Key();
        public static Key Enter { get; } = new Key();
        public static Key Space { get; } = new Key();
        public static Key Tab { get; } = new Key();
        public static Key Shift { get; } = new Key();
        public static Key Control { get; } = new Key();
        public static Key Back { get; } = new Key();
        public static Key Delete { get; } = new Key();
    }
}
