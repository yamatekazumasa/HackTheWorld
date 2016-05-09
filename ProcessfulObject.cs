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
        private List<Process> _processes;
        private readonly IEnumerator _routine;
        private float _dt;

        public ProcessfulObject() : base(500, 300)
        {
            _routine = GetEnumerator();
        }
        public ProcessfulObject(int x, int y) : base(x, y)
        {
            _routine = GetEnumerator();
        }
        public ProcessfulObject(int x, int y, int w, int h) : base(x, y, 0, 0, w, h)
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
            _processes = processes.ToList();
        }

        public void SetProcesses(List<Process> processes)
        {
            _processes = processes;
        }

        public void AddProcess(Process process)
        {
            _processes.Add(process);
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Turquoise, MinX, MinY, Width, Height);
        }
    }
}
