using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;
using System.Drawing;

namespace HackTheWorld
{
    public static class Input
    {
        public class Key
        {
            private uint _history;

            public void Append(uint s)
            {
                _history = (_history << 1) | s;
            }

            public bool Pressed => (_history & 0x01) > 0;
            public bool Pushed => !((_history & 0x02) > 0) && ((_history & 0x01) > 0);
        }

        public class MouseButton
        {
            private uint _history;

            public void Append(uint s)
            {
                _history = (_history << 1) | (uint) (s > 0 ? 1 : 0);
            }

            public bool Pressed => (_history & 0x01) > 0;
            public bool Pushed => !((_history & 0x02) > 0) && ((_history & 0x01) > 0);
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


        public static void Update(LinkedList<Keys> pressedKeys)
        {
            Up.Append((uint)(pressedKeys.Contains(Keys.Up) ? 1 : 0));
            Down.Append((uint)(pressedKeys.Contains(Keys.Down) ? 1 : 0));
            Left.Append((uint)(pressedKeys.Contains(Keys.Left) ? 1 : 0));
            Right.Append((uint)(pressedKeys.Contains(Keys.Right) ? 1 : 0));
            Sp1.Append((uint)(pressedKeys.Contains(Keys.Z) ? 1 : 0));
            Sp2.Append((uint)(pressedKeys.Contains(Keys.X) ? 1 : 0));
            Sp3.Append((uint)(pressedKeys.Contains(Keys.C) ? 1 : 0));
            Enter.Append((uint)(pressedKeys.Contains(Keys.Enter) ? 1 : 0));
            Space.Append((uint)(pressedKeys.Contains(Keys.Space) ? 1 : 0));
            Tab.Append((uint)(pressedKeys.Contains(Keys.Tab) ? 1 : 0));
            Shift.Append((uint)(pressedKeys.Contains(Keys.Shift) ? 1 : 0));
            Control.Append((uint)(pressedKeys.Contains(Keys.Control) ? 1 : 0));
            Back.Append((uint)(pressedKeys.Contains(Keys.Back) ? 1 : 0));
            Delete.Append((uint)(pressedKeys.Contains(Keys.Delete) ? 1 : 0));
        }

        public static void Update(LinkedList<MouseButtons> mouseButtons)
        {
            LeftButton.Append((uint)(mouseButtons.Contains(MouseButtons.Left) ? 1 : 0));
            RightButton.Append((uint)(mouseButtons.Contains(MouseButtons.Right) ? 1 : 0));
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

    }
}
