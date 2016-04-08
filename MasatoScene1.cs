using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MasatoScene1 : Scene
    {
        //このSceneで使用する変数
        Image _img;
        Player player;

        public override void Cleanup()//終了時処理
        {
        }

        public override void Startup()//初期化処理
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            player = new Player(new Vector(100, 100));
        }

        public override void Update()
        {
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }
            //ここに作成
            // 計算とか

            player.move();


            GraphicsContext.Clear(Color.White);
            //GraphicsContext.DrawImage(_img, 0, 0);

            //ここに作成
            // 描画のみ

            player.draw(_img);


        }
    }
}
