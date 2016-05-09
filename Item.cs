using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Item : GameObject
    {
        public Item(int x, int y) : base(x, y)
        {
            ObjectType = ObjectType.Item;
        }

        public Item(int x, int y, int vx, int vy) : base(x, y, vx, vy)
        {
            ObjectType = ObjectType.Item;
        }

        public Item(int x, int y, int vx, int vy, int w, int h) : base(x, y, vx, vy, w, h)
        {
            ObjectType = ObjectType.Item;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.GreenYellow, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.LawnGreen, X, Y, Width, Height);
        }
    }
}
