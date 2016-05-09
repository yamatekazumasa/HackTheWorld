using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    static partial class Constants
    {
        public static int[,] Map = 
        {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,23, 0, 0 ,0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13 ,1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1 ,1},
            {1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 ,0},
            {2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 ,0},
            {0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 ,0},
            {1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0 ,0},
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0 ,0},
            {0, 0, 0, 0, 0, 0, 0,11, 0, 0, 0, 0, 0, 0, 0 ,0},
            {0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2 ,0},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1}
        };

        /// <summary>
        /// ゲーム画面のマスの個数。
        /// </summary>
        public static readonly int CellNumX = 16;
        public static readonly int CellNumY = 12;

    }
}
