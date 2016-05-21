using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    /// <summary>
    /// 編集可能な敵。
    /// IEditable.SetDemoProcesses()によって動作を付与している。
    /// </summary>
    class EditableEnemy : Enemy, IEditable
    {
        public int ProcessPtr { get; set; }
        public CodeBox Codebox { get; set; }
        public List<Process> Processes { get; set; }
        public bool CanExecute { get; set; }

        public EditableEnemy(float x, float y) : base(x, y) { }

        public EditableEnemy(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public EditableEnemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            CanExecute = false;
            Codebox = new CodeBox(this) { Position = Position + new Vector(50, -50) };
        }

        public override void Update(float dt)
        {
            ((IEditable)this).Update(dt);
        }

    }
}
