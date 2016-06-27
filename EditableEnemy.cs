using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// 編集可能な敵。
    /// IEditable.SetDemoProcesses()によって動作を付与している。
    /// </summary>
    class EditableEnemy : Enemy, IEditable
    {
        /// <summary>
        /// 何番目の Process が実行されているか。
        /// </summary>
        public int ProcessPtr { get; set; }
        /// <summary>
        /// 自身のコード。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 自身の動作を格納する。
        /// </summary>
        public List<Process> Processes { get; set; }
        /// <summary>
        /// true のとき Update() 内で Process が実行されるようになる。
        /// </summary>
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
