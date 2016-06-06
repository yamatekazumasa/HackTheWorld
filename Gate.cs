using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Gate : GameObject
    {
        [JsonProperty("next", Order = 0)]
        public string NextStage { get; set; }

        public Gate(float x, float y) : base(x, y) {}

        public override void Initialize()
        {
            base.Initialize();
            ObjectType = Constants.ObjectType.Gate;
        }

        public override void Draw()
        {
            Constants.GraphicsContext.DrawRectangle(Pens.Black, this);
        }
    }
}
