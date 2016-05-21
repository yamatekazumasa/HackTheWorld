using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// IEditable な Block
    /// </summary>
    public class PBlock : Block, IEditable
    {
        public int ProcessPtr { get; set; }
        public CodeBox Codebox { get; set; }
        public List<Process> Processes { get; set; }
        public bool CanExecute { get; set; }

        [JsonProperty("code", Order = 10)]
        public string Code => Codebox.Current.Text.ToString();
        public bool IsWorking = false;

        public PBlock(float x, float y) : base(x, y) { }

        public override void Initialize()
        {
            base.Initialize();
            CanExecute = false;
            Codebox = new CodeBox(this) { Position = Position + new Vector(70, -70) };
            Processes = new List<Process>();
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
