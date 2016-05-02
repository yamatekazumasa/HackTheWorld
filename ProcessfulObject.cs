using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IEnumerator _routine;
        private float _dt;

        public ProcessfulObject() : base(500, 300, 100, 100)
        {
            _routine = GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            Stopwatch stopwatch = new Stopwatch();
            foreach (var process in _processes)
            {
                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < process.MilliSeconds)
                {
                    process.ExecuteWith(this, _dt);
                    yield return null;
                }
            }

        }

        public override void Update(float dt)
        {
            _dt = dt;
            _routine.MoveNext();
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
