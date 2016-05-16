using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EditableObject : GameObject, IEnumerable
    {
        private List<Process> _processes;
        private IEnumerator _routine;
        private float _dt;
        private CodeBox _codebox;

        public bool IsFocused => _codebox.IsFocused;
        [JsonProperty("code", Order = 10)]
        public string Code => _codebox.Current.Text.ToString();
        public bool CanExecute => _routine != null && Scene.Current is GameScene;

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
            foreach (var process in _processes)
            {
                float elapsedTime = 0;
                while (elapsedTime * 1000 < process.MilliSeconds)
                {
                    process.ExecuteWith(this, _dt);
                    elapsedTime += _dt;
                    yield return null;
                }
            }
        }

        public void Compile()
        {
            // ここにstring型をProcess型に変換する処理を書く。
            string str = _codebox.GetString();
            // SetProcesses(new Process[] {});
            if (str.Contains("this.X += 1")) X += CellSize;
            _routine = GetEnumerator();
        }

        public override void Update(float dt)
        {
            _dt = dt;
            if (CanExecute) _routine.MoveNext();
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
