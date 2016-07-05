using System.Collections.Generic;
using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 編集可能な敵。
    /// </summary>
    class EditableEnemy : Enemy, IEditable
    {
        // IEditable のプロパティ
        public int ProcessPtr { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<Process> Processes { get; set; }
        public bool CanExecute { get; set; }

        public EditableEnemy(float x, float y) : base(x, y) { }

        public EditableEnemy(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public EditableEnemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

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
            GraphicsContext.FillRectangle(Brushes.Turquoise, MinX, MinY, Width, Height);
        }
    }
}
