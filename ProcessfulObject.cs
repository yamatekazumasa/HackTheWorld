using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class ProcessfulObject : GameObject, IEnumerable
    {
        private Process[] _processes;

        public ProcessfulObject() : base(500, 300, 100, 100)
        {
            
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var process in _processes)
            {
                for (var i = 0; i < process.Frame; i++)
                {
                    process.ExecuteWith(this);
                    yield return null;
                }
            }

        }

        public void SetProcesses(Process[] processes)
        {
            _processes = processes;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Turquoise, MinX, MinY, Width, Height);
        }
    }
}
