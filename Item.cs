using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// アイテム
    /// </summary>
    public class Item : GameObject
    {
        [JsonProperty("effect", Order = 20)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemEffects ItemEffect;

        public Item(float x, float y, ItemEffects effect) : base(x, y)
        {
            ItemEffect = effect;
        }

        public Item(float x, float y, float vx, float vy, ItemEffects effect) : base(x, y, vx, vy)
        {
            ItemEffect = effect;
        }

        public Item(float x, float y, float vx, float vy, float w, float h, ItemEffects effect) : base(x, y, vx, vy, w, h)
        {
            ItemEffect = effect;
        }

        public override void Initialize()
        {
            base.Initialize();
            W = CellSize / 2;
            H = CellSize / 2;
            ObjectType = ObjectType.Item;
        }

        public void GainedBy(GameObject obj)
        {
            if (ItemEffect == ItemEffects.Bigger)
            {
                obj.Y -= CellSize / 4;
                obj.Height += CellSize / 4;
                obj.Width = CellSize;
                if (obj is Player) ((Player)obj).Jumpspeed = -CellSize * 13; // h=v^2/2g
            }
            Die();
        }

        public override void Draw()
        {
            if (!IsAlive) return;
            GraphicsContext.FillRectangle(Brushes.GreenYellow, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.ForestGreen, X, Y, Width, Height);
        }
    }
}
