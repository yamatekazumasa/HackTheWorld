﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HackTheWorld
{
    public static partial class Constants
    {
        /// <summary>
        /// FPS(Frame per second)。
        /// </summary>
        public static readonly int Fps = 60;

        /// <summary>
        /// スケール。
        /// </summary>
        public static readonly int Scale = 100;

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
            Player, Enemy, Item, Block
        }

        public static Graphics GraphicsContext;

    }
}
