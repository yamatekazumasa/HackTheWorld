using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MenuItem : GameObject
    {
        private readonly Image[] _images;
        public bool _selected = false;

        public MenuItem(Image defaultImage, Image selectedImage)
        {
            _images = new Image[2] {defaultImage, selectedImage};
            SetSize(defaultImage.Width, defaultImage.Height);
        }

        public override void Draw()
        {
            var img = _selected ? _images[1] : _images[0];
            GraphicsContext.DrawImage(img, GetMinX(), GetMinY());
        }

    }
}
