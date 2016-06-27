using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// メニューのボタン用のクラス。
    /// 通常時の画像と選択されているときの画像を二枚用意して使い分ける。
    /// </summary>
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
