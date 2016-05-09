using System.Collections.Generic;
using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Item : GameObject
    {
        public int number = 0;
        public Item(int x, int y) : base(x, y)
        {
        }

        public Item(int x, int y, int vx, int vy) : base(x, y, vx, vy)
        {
        }

        public Item(int x, int y, int vx, int vy, int w, int h) : base(x, y, vx, vy, w, h)
        {
        }

        public virtual void Effect(Player player, List<GameObject> list)
        {
            switch (number)
            {
                case 1:
                    player.Y -= CellSize / 4;
                    player.Height += CellSize / 4;
                    player.Width = CellSize;
                    break;
                case 2:
                    foreach (var obj in list)
                    {
                        if(obj.Type == ObjectType.Block)
                        {
                            obj.Type = ObjectType.Item;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Pink, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.Magenta   , X, Y, Width, Height);
        }
    }
}
