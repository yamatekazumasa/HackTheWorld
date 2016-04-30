using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    public static partial class Extensions
    {
        public static Vector ToVector(this Point p)
        {
            return new Vector(p.X, p.Y);
        }

        public static Vector ToVector(this Size s)
        {
            return new Vector(s.Width, s.Height);
        }

        /// <summary>
        /// ディープコピーを作成する。
        /// クローンするクラスには Serializable 属性、
        /// 不要なフィールドは NonSerialized 属性をつける。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Clone<T>(this T target)
        {
            object clone = null;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, target);
                stream.Position = 0;
                clone = formatter.Deserialize(stream);
            }
            return (T)clone;
        }
    }
}
