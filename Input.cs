using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HackTheWorld
{
    public static class Input
    {
        public class Key
        {
            private bool _wasPressed;
            private bool _isPressed;

            public void Append(bool isPressed)
            {
                _wasPressed = _isPressed;
                _isPressed = isPressed;
            }

            public bool Pressed => _isPressed;
            public bool Pushed => !_wasPressed && _isPressed;
        }

        public class MouseButton
        {
            private bool _wasPressed;
            private bool _isPressed;

            public void Append(bool isPressed)
            {
                _wasPressed = _isPressed;
                _isPressed = isPressed;
            }

            public bool Pressed => _isPressed;
            public bool Pushed => !_wasPressed && _isPressed;
        }

        public class MousePosition
        {
            private Vector _position;

            public void Append(Vector mousePosition, Vector windowPosition)
            {
                _position = mousePosition - windowPosition - new Vector(9, 30);

            }

            public int X => (int)_position.X;
            public int Y => (int)_position.Y;
            public Vector Position => _position;
        }


        public class KeyBoards
        {
            char _buffer;

            public void Append(char c)
            {
                _buffer = c;
            }

            public void Clear()
            {
                _buffer = '\0';
            }

            public bool IsDefined => _buffer != '\0';

            public char TypedChar => _buffer;

        }

        public static void Update(LinkedList<Keys> pressedKeys)
        {
            Up.Append(pressedKeys.Contains(Keys.Up));
            Down.Append(pressedKeys.Contains(Keys.Down));
            Left.Append(pressedKeys.Contains(Keys.Left));
            Right.Append(pressedKeys.Contains(Keys.Right));
            Sp1.Append(pressedKeys.Contains(Keys.Z));
            Sp2.Append(pressedKeys.Contains(Keys.X));
            Sp3.Append(pressedKeys.Contains(Keys.C));
            A.Append(pressedKeys.Contains(Keys.A));
            W.Append(pressedKeys.Contains(Keys.W));
            Y.Append(pressedKeys.Contains(Keys.Y));
            Enter.Append(pressedKeys.Contains(Keys.Enter));
            Space.Append(pressedKeys.Contains(Keys.Space));
            Tab.Append(pressedKeys.Contains(Keys.Tab));
            Shift.Append(pressedKeys.Contains(Keys.ShiftKey));
            Control.Append(pressedKeys.Contains(Keys.ControlKey));
            Back.Append(pressedKeys.Contains(Keys.Back));
            Delete.Append(pressedKeys.Contains(Keys.Delete));
        }

        public static void Update(LinkedList<MouseButtons> mouseButtons)
        {
            LeftButton.Append(mouseButtons.Contains(MouseButtons.Left));
            RightButton.Append(mouseButtons.Contains(MouseButtons.Right));
        }

        public static void Update(Point mousePosition, Point windowPosition)
        {
            Mouse.Append(mousePosition.ToVector(), windowPosition.ToVector());
        }

        public static Key Up { get; } = new Key();
        public static Key Down { get; } = new Key();
        public static Key Left { get; } = new Key();
        public static Key Right { get; } = new Key();
        public static Key Sp1 { get; } = new Key();
        public static Key Sp2 { get; } = new Key();
        public static Key Sp3 { get; } = new Key();
        public static Key A { get; } = new Key();
        public static Key Y { get; } = new Key();
        public static Key W { get; } = new Key();
        public static Key Enter { get; } = new Key();
        public static Key Space { get; } = new Key();
        public static Key Tab { get; } = new Key();
        public static Key Shift { get; } = new Key();
        public static Key Control { get; } = new Key();
        public static Key Back { get; } = new Key();
        public static Key Delete { get; } = new Key();

        public static MousePosition Mouse { get; } = new MousePosition();
        public static MouseButton LeftButton { get; } = new MouseButton();
        public static MouseButton RightButton { get; } = new MouseButton();
        public static KeyBoards KeyBoard { get; } = new KeyBoards();
        
    }
}
