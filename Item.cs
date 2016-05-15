using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Item : GameObject
    {
        public Item(int x, int y) : base(x, y)
        {
        }

        //Itemを取得した時のPlayerのサイズを変更する
        public void Giant(Player player)
        {
            player.Height *= 2;
        }

        public override void Draw()
        {
        GraphicsContext.FillRectangle(Brushes.Brown, X, Y, Width, Height);
        GraphicsContext.DrawRectangle(Pens.Black, X, Y, Width, Height);
        }
    }
}
