using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public class PBlock : Block, IEditable
    {
        public float Dt { get; set; }
        public CodeBox Codebox { get; private set; }
        public List<Process> Processes { get; set; }
        public IEnumerator Routine { get; set; }

        [JsonProperty("code", Order = 10)]
        public string Code => Codebox.Current.Text.ToString();
        public bool IsWorking = false;

        public PBlock(float x, float y) : base(x, y) { }

        public override void Initialize()
        {
            base.Initialize();
            _isEditable = true;
            Codebox = new CodeBox(this) { Position = Position + new Vector(100, 50) };
        }

        public override void Update(float dt)
        {
            ((IEditable)this).Update(dt);
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Gold, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Black, X, Y, Width, Height);
            Codebox.Draw();
        }
    }
}
