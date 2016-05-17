using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    class EnemyPacpac : Enemy
    {
        public override void Draw()
        {
            GraphicsContext.FillPie(Brushes.Blue, X, Y, Width, Height, 0, 360);
        }
    }
}
