﻿using System;
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

        public class Mouse
        {
            private uint _history;

            public void Append(uint s)
            {
                _history = (_history << 1) | (uint)(s > 0 ? 1 : 0);
            }

            public bool Pressed => (_history & 0x01) > 0;
            public bool Pushed => !((_history & 0x02) > 0) && ((_history & 0x01) > 0);
        }
        public class mousePosition
        {
            private Point cp; private Point sp;
            internal Point position;
            public void setpoint(Point cp, Point sp)
            {
                this.cp = cp;
                this.sp = sp;
                var x = sp.X - cp.X-9;
                var y = sp.Y - cp.Y-30;
                position = new Point(x, y);
            }
         
        }



        public static void Update(LinkedList<Keys> pressedKeys)
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


        public static void Update(List<MouseButtons> mouseButtons)
        {
            MouseLeft.Append((uint)(mouseButtons.Contains(MouseButtons.Left) ? 1 : 0));
            MouseRight.Append((uint)(mouseButtons.Contains(MouseButtons.Right) ? 1 : 0));
          
        }
        public static void Update(Point cp, Point sp)
        {
            mp.setpoint(cp,sp);
        }

    


        public static Mouse MouseLeft { get; } = new Mouse();
        public static Mouse MouseRight { get; } = new Mouse();

        public static mousePosition mp = new mousePosition();

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
