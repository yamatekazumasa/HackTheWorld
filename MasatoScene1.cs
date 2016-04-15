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
        GameObject player;
        List<GameObject> blocks;

        public override void Cleanup()//終了時処理
        {
        }

        public override void Startup()//初期化処理
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            // playerの初期化
            player = new GameObject(1000, 400);
            player.Initialize(ObjectType.Player);
            // ブロックの初期化
            blocks = new List<GameObject>();
            for (int i=0;i<5; i++){
                GameObject b = new GameObject(300*i+50, 500+50);
                b.Initialize(ObjectType.Block);
                blocks.Add(b);
            }
        }

        public override void Update()
        {
            if (Input.Sp2.Pushed)
            {
                Scene.Pop();
            }
            //ここに作成
            // 計算とか

            player.MovebyKeys(10);//playerのスピード
            foreach (var block in blocks)
            {
                player.Adjust(block);
            }

            GraphicsContext.Clear(Color.White);
            //GraphicsContext.DrawImage(_img, 0, 0);

            //ここに作成
            // 描画のみ

            player.DrawImage(_img);
            foreach (var block in blocks) {
                block.Draw(Brushes.Brown);
            }

        }
    }
}
