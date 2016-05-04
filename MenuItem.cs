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
        public bool IsSelected { get; set; } = false;

        public MenuItem(Image defaultImage, Image selectedImage)
        {
            _images = new Image[2] {defaultImage, selectedImage};
            Size = new Vector(defaultImage.Width, defaultImage.Height);
        }
        public MenuItem(Image defaultImage)
        {

            _images = new Image[2] { defaultImage, defaultImage };
            Size = new Vector(defaultImage.Width, defaultImage.Height);
        }

        public override void Draw()
        {
            var img = IsSelected ? _images[1] : _images[0];
            GraphicsContext.DrawImage(img, this);
        }

    }
}
