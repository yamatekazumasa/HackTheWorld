using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HackTheWorld
{
    /// <summary>
    /// 各種定数を格納する。
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// FPS(Frame per second)。
        /// </summary>
        public static readonly int Fps = 60;

        /// <summary>
        /// スケール。
        /// </summary>
        public static readonly int Scale = 100000;

        /// <summary>
        /// ウィンドウサイズ。
        /// </summary>
        public static readonly int ScreenWidth = 1280;
        public static readonly int ScreenHeight = 720;

        /// <summary>
        /// 格子のサイズ。
        /// </summary>
        public static readonly int CellSize = 60;

        /// <summary>
        /// 重力の大きさ。
        /// </summary>
        public static readonly int Gravity = CellSize * 25;

        /// <summary>
        /// オブジェクトのタイプ。
        /// </summary>
        public enum ObjectType
        {
            Player=0, Block=1, Enemy=2, Item=3,
            Gate=100
        }

        /// <summary>
        /// アイテムのタイプ。
        /// </summary>
        public enum ItemEffects
        {
            Bigger, Smaller
        }

        /// <summary>
        /// どこからでも描画できるようにするために使っている。
        /// 同時に別スレッドからアクセスさせると落ちるので、これに対して非同期な処理は行わないように。
        /// </summary>
        public static Graphics GraphicsContext;

        /// <summary>
        /// クリップボードをいじるのに必要だった。
        /// </summary>
        public static Form WindowContext;

    }
}
