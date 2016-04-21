using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Player : GameObject
    {
        public Player(int x, int y) : base(x, y)
        {
        }

        /// <summary>
        /// 押されたキー(上下左右)により1フレーム分動く。
        /// </summary>
        public void MovebyKeys(int speed)
        {
            speed /= 10;    // スケール調整
            if (Input.Left.Pressed)  this.SetPosition(GetPosition() + new Vector(-speed, 0));
            if (Input.Right.Pressed) this.SetPosition(GetPosition() + new Vector(+speed, 0));
            //if (Input.Up.Pressed) this._position += new Vector(0, -speed);
            //if (Input.Down.Pressed) this._position += new Vector(0, +speed);
        }

        /// <summary>
        /// 押されたキー(上)によりvelocityが更新される。
        /// </summary>
        public void JumpbyKeys(int speed)
        {
            if (Input.Sp1.Pushed) this.SetVelocity(0, -speed);
        }
    }
}
