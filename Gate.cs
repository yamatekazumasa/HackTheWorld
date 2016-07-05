using System.Drawing;
using Newtonsoft.Json;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Gate : GameObject
    {
        [JsonProperty("next", Order = 0)]
        public string NextStage { get; set; }

        public Gate(float x, float y) : base(x, y) {}

        public override void Draw()
        {
            Constants.GraphicsContext.DrawRectangle(Pens.Black, this);
        }
    }
}
