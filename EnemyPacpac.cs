using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class EnemyPacpac : Enemy
    {
        public EnemyPacpac(float x, float y) : base(x, y)
        {
        }

        public override void Draw()
        {
            GraphicsContext.FillPie(Brushes.Blue, X, Y, Width, Height, 0, 360);
        }
    }
}
