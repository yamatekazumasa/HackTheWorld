using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class MasatoScene1 : Scene
    {
        Image _img;
        Player _player;
        List<GameObject> _blocks;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _img = Image.FromFile(@"image\masato1.jpg");
            // playerの初期化
            _player = new Player(_img);
            _player.Initialize(ObjectType.Player);
            // ブロックの初期化
            _blocks = new List<GameObject>();
            for (int i=0;i<5; i+=2){
                GameObject b = new GameObject(Cell*i, Cell*10);
                b.Initialize(ObjectType.Block);
                _blocks.Add(b);

            }
        }

        public override void Update()
        {
            if (Input.Sp2.Pushed||Input.MouseLeft.Pushed)
            {
                Scene.Pop();
            }

            _player.Update();

            foreach (var block in _blocks)
            {
                if (_player.Intersects(block))
                {
                    _player.VY = 0;
                }
                _player.Adjust(block);
            }
            
            GraphicsContext.Clear(Color.White);

            //GraphicsContext.DrawImage(_img, 0, 0);

            //ここに作成
            // 描画のみ
            for (int ix=0;ix<ScreenWidth;ix+=Cell)
            {
                GraphicsContext.DrawLine(Pens.Gray, ix, 0, ix, ScreenHeight);
            }
            for (int iy = 0; iy < ScreenHeight; iy += Cell)
            {
                GraphicsContext.DrawLine(Pens.Gray, 0, iy, ScreenWidth, iy);
            }


            _player.Draw();
            foreach (var block in _blocks)
            {
                block.Draw();

            }

        }
    }
}
