using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    public interface IControllable
    {
        Vector Position { get; set; }
    }

    public static class Controllable
    {
        /// <summary>
        /// 押されたキー(上下左右)の方向に speed 分動く。
        /// </summary>
        /// <param name="speed"></param>
        public static void MoveByKeys(this IControllable obj, int speed)
        {
            if (Input.Left.Pressed)  obj.Position += new Vector(-speed, 0);
            if (Input.Right.Pressed) obj.Position += new Vector(+speed, 0);
            if (Input.Up.Pressed)    obj.Position += new Vector(0, -speed);
            if (Input.Down.Pressed)  obj.Position += new Vector(0, +speed);
        }
    }

}
