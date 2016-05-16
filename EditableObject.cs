using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    class EditableObject : GameObject, IEnumerable
    {
        private List<Process> _processes;
        private IEnumerator _routine;
        private float _dt;
        private CodeBox _codebox;

        public bool IsFocused => _codebox.IsFocused;
        public bool IsExecutable => _processes != null;
        [JsonProperty("code", Order = 10)]
        public string Code => _codebox.Current.Text.ToString();

        public EditableObject() : base(500, 300) { }
        public EditableObject(float x, float y) : base(x, y) { }
        public EditableObject(float x, float y, float w, float h) : base(x, y, 0, 0, w, h) { }
        public EditableObject(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            _codebox = new CodeBox(this) {Position = Position + new Vector(100, 50)};
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

        public void Compile()
        {
            string str = _codebox.GetString();
            // ここにstring型をProcess型に変換する処理を書く。
            SetProcesses(new Process[] {});
            _routine = GetEnumerator();
        }

        public override void Update(float dt)
        {
            _dt = dt;
            if (IsExecutable) _routine.MoveNext();
            if (Clicked) _codebox.Focus();
            _codebox.Update();
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
            _codebox.Draw();
        }

    }
}
