using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public static class Input
    {
        public class Key
        {
            private uint _history;

            public void Append(uint s)
            {
                _history = (_history << 1) | (uint)(s > 0 ? 1 : 0);
            }

            public bool Pressed => (_history & 0x01) > 0;
            public bool Pushed => !((_history & 0x02) > 0) && ((_history & 0x01) > 0);
        }

        public static void Update(List<Keys> pressedKeys)
        {
            Up.Append((uint)(pressedKeys.Contains(Keys.Up) ? 1 : 0));
            Down.Append((uint)(pressedKeys.Contains(Keys.Down) ? 1 : 0));
            Left.Append((uint)(pressedKeys.Contains(Keys.Left) ? 1 : 0));
            Right.Append((uint)(pressedKeys.Contains(Keys.Right) ? 1 : 0));
            Button1.Append((uint)(pressedKeys.Contains(Keys.Enter) ? 1 : 0));
            Button2.Append((uint)(pressedKeys.Contains(Keys.Space) ? 1 : 0));
            Sp1.Append((uint)(pressedKeys.Contains(Keys.Z) ? 1 : 0));
            Sp2.Append((uint)(pressedKeys.Contains(Keys.X) ? 1 : 0));
        }

        public static Key Up { get; } = new Key();
        public static Key Down { get; } = new Key();
        public static Key Left { get; } = new Key();
        public static Key Right { get; } = new Key();
        public static Key Button1 { get; } = new Key();
        public static Key Button2 { get; } = new Key();
        public static Key Sp1 { get; } = new Key();
        public static Key Sp2 { get; } = new Key();
    }
}
