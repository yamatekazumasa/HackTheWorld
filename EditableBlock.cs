using System.Collections.Generic;
using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 編集可能なブロック。
    /// </summary>
    public class EditableBlock : Block, IEditable
    {
        // IEditable のプロパティ
        public int ProcessPtr { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<Process> Processes { get; set; }
        public bool CanExecute { get; set; }

        public bool IsWorking = false;

        public EditableBlock(float x, float y) : base(x, y) { }

        public override void Initialize()
        {
            base.Initialize();
            CanExecute = false;
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
        }
    }
}
