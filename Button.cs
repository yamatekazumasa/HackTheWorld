using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class Button:GameObject
    {
        private readonly Image image;
        public Button(Image image)
        {
            this.image = image;
            SetSize(image.Width, image.Height);
        }
        public bool clicked(Point mousepoint,bool mouseleft)
        {
            if (this.Intersects(mousepoint) && mouseleft) return true;
            else return false;
        }
        public override void Draw()
        {
            GraphicsContext.DrawImage(image, MinX,MinY,(float)Size.X,(float)Size.Y);
        }
    }
}
