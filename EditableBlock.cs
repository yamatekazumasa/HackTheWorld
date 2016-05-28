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
    public class EditableBlock : Block, IEditable
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
        public bool IsWorking = false;

        public EditableBlock(float x, float y) : base(x, y) { }

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
