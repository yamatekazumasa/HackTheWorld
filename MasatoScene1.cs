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
            player = new GameObject(50, 500);
            player.Initialize(ObjectType.Player);
            player.SetSize(70,90);  // プレーヤーサイズ(仮) 
            // ブロックの初期化
            blocks = new List<GameObject>();
            for (int i=0;i<15; i++){
                GameObject b = new GameObject(100*i+50, 700+50);
                b.Initialize(ObjectType.Block);
                blocks.Add(b);
            }
            for (int i = 0; i < 5; i++)
            {
                GameObject b = new GameObject(300 * i + 50, 100*i + 150);
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

            // 計算

            // 移動する前に行う計算
            foreach (var block in blocks)
            {
                if (player.StandOn(block) || block.StandOn(player))
                {
                    player.SetVelocity(0, 0);
                    player.JumpbyKeys(50);// ジャンプの初速
                }
            }
            player.Accelerate(new Vector(0, 5));// 重力

            // 移動の計算
            player.Move();
            player.MovebyKeys(20);//playerのスピード

            // 移動後の調整
            foreach (var block in blocks)
            {
                player.Adjust(block);
            }

            // 画面のクリア
            GraphicsContext.Clear(Color.White);

            // 描画
            player.DrawImage(_img);
            //player.Draw(Brushes.Aqua);
            foreach (var block in blocks) {
                block.Draw(Brushes.Brown);
            }

        }
    }
}
