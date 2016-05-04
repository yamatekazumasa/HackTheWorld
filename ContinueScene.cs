using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;
using System.Drawing;


namespace HackTheWorld
{
    class ContinueScene : Scene
    {
        //画像を読み込む
        Bitmap bmp = new Bitmap(@"image\gameover.bmp");

        private readonly MenuItem _continueButton = new MenuItem(Image.FromFile(@"image\continue.bmp"), Image.FromFile(@"image\continue1.bmp"));
        private readonly MenuItem _closeButton = new MenuItem(Image.FromFile(@"image\close1.bmp"), Image.FromFile(@"image\close.bmp"));
        List<MenuItem> menuItem = new List<MenuItem>();
        public override void Cleanup()
        {
        }
        public override void Startup()
        {
            _continueButton.Size = new Vector(400, 100);
            _continueButton.Position = new Vector(800, 200);
            _closeButton.Size = new Vector(400, 100);
            _closeButton.Position = new Vector(800, 300);
            menuItem.Add(_continueButton); menuItem.Add(_closeButton);
         //   System.Threading.Thread.Sleep(1000);



        }
        public override void Update(float dt)
        {
            //    //ColorMatrixオブジェクトの作成
            //    System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
            //    //ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
            //    cm.Matrix00 = 1;
            //    cm.Matrix11 = 1;
            //    cm.Matrix22 = 1;
            //    cm.Matrix33 = 0.5F;
            //    cm.Matrix44 = 1;
            //    //ImageAttributesオブジェクトの作成
            //    System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
            //    //ColorMatrixを設定する
            //    ia.SetColorMatrix(cm);
            //    //ImageAttributesを使用して画像を描画

            //背景を透明にする
            bmp.MakeTransparent();
            GraphicsContext.DrawImage(bmp,  0, 0);

            foreach (var button in menuItem)
            {
                button.IsSelected = false;
                if (button.Contains(Input.Mouse.Position)) button.IsSelected = true;
            }
            if (_continueButton.Clicked)
            {
                Scene.Pop();
                Scene.Pop();
                Scene.Push(new MasatoScene1());
            }
            if (_closeButton.Clicked)
            {
                Scene.Pop();
                Scene.Pop();
            }
            foreach (var item in menuItem)
            {
                item.Draw();
            }
        }
    }
}