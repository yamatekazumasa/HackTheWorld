using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// IEditable のテスト用
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EditableObject : GameObject, IEditable
    {
        /// <summary>
        /// 何番目の Process が実行されているか。
        /// </summary>
        public int ProcessPtr { get; set; }
        /// <summary>
        /// 自身のコードを編集するテキストエディタ。
        /// </summary>
        public CodeBox Codebox { get; set; }
        /// <summary>
        /// 自身の動作を格納する。
        /// </summary>
        public List<Process> Processes { get; set; }
        /// <summary>
        /// true のとき Update() 内で Process が実行されるようになる。
        /// </summary>
        public bool CanExecute { get; set; }

        [JsonProperty("code", Order = 10)]
        public string Code => Codebox.Current.Text.ToString();

        public EditableObject() : base(500, 300) { }
        public EditableObject(float x, float y) : base(x, y) { }
        public EditableObject(float x, float y, float w, float h) : base(x, y, 0, 0, w, h) { }
        public EditableObject(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            CanExecute = false;
            Codebox = new CodeBox(this) {Position = Position + new Vector(50, -50)};
            Processes = new List<Process>();
        }

        public override void Update(float dt)
        {
            ((IEditable)this).Update(dt);
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Turquoise, MinX, MinY, Width, Height);
            Codebox.Draw();
        }

    }
}
