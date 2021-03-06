﻿using System;
using System.Drawing;
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


        public static Func<int> CreateCounter()
        {
            var i = 0;
            return () => i++;
        }

        public static Func<int> Counter = CreateCounter();

    }
}
