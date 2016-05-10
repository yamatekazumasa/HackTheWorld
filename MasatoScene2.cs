using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;
using static HackTheWorld.Kakikae;

namespace HackTheWorld
{
    class MasatoScene2 : Scene
    {
        Image _img;
        private readonly MenuItem _backButton = new MenuItem(Image.FromFile(@"image\back.png"));
        private readonly MenuItem _masato3Button = new MenuItem(Image.FromFile(@"image\masato3.jpg"));
        private MenuItem _runButton;
        private CodeBox _box;
        private ProcessfulObject pfo;
        private bool _run;
        private string str;
        public override void Cleanup( )
        {
        }


        public override void Startup( )
        {
            _box = new CodeBox( );
            _box.Position = new Vector(400 , 20);
            _img = Image.FromFile(@".\image\masato2.jpg");

            _backButton.Size = new Vector(50 , 50);
            _backButton.Position = new Vector(25 , 500);

            _masato3Button.Size = new Vector(50 , 50);
            _masato3Button.Position = new Vector(75 , 500);

            _runButton = new MenuItem(Image.FromFile(@"image\masato3.jpg"))
            {
                Size = new Vector(50 , 50),
            Position = new Vector(125 , 500)
        };
            _run = false;
            str = "";
        }

        public override void Update(float dt)
        {
            if(_backButton.Clicked) Scene.Pop( );
            
            if(_runButton.Clicked)
            {
                if(_run == false)
                {
                    //文字列をkakikae.csにもってく
                    _run = true;
                    string _s = _box.GetString( );
                    str = yomitori(_s);
                }
                else
                {
                    _run = false;
                    str = "";
                }
            }
            pfo = new ProcessfulObject( );
            makeprocess(pfo,str);

            if(_masato3Button.Clicked) Scene.Push(new MasatoScene3( ));
            if(Input.Sp2.Pushed && !_box.IsFocused) Scene.Pop( );
            if(Input.KeyBoard.IsDefined) _box.Insert(Input.KeyBoard.TypedChar);


            _box.Update( );

            GraphicsContext.Clear(Color.White);
            if(_run)
            {
                GraphicsContext.DrawString(str , new Font("Arial" , 12) , Brushes.Black , new Rectangle(500 , 300 , 500 , 300));
            }
                GraphicsContext.DrawImage(_img , 0 , 0);
            _box.Draw( );
            _backButton.Draw( );
            _masato3Button.Draw( );
            _runButton.Draw( );
            //GraphicsContext.DrawString(_box.GetString(), new Font("Arial" , 12) , Brushes.Black , new Rectangle(500 , 300 , 500 , 300));
        }
        //プロセスを作れるかためす
        public void makeprocess(ProcessfulObject pfo , string s1)
        {
            char[ ] delimiterChars = { ' ' , ',' , '.' , ':' , '\t' , '\n' };

            ArrayList sArray = new ArrayList( );
            string[ ] s2 = s1.Split(delimiterChars);
            for(int i = 0; i < s2.Length; i++)
            {
                sArray.Add(s2[i]);
            }
            for(int i = 0; i < sArray.Count; i++)
            {
                switch((string)sArray[i])
                {
                    case "move()":
                        pfo.SetProcesses(new Process[ ]
                        {
                            new Process((obj,dt)=> { obj.Size = new Vector(30, 30); }, 2.0f) //doubleをfloatにするときに2.0fじゃないと認識しない または(float)をつける
                        });
                        break;
                      
                   /* 
                     move()だけなら良いけど、引数が来た時にどうするか
                     「move(x,y)」と書かせて、moveを読み取ってx,yをVector型で保存
                     
                    case "move":
                        最初...xとyを読み取る
                        x,yをVector型にする
                        AddProcess((obj,dt)=> { obj.Size = new Vector(x, y)}, sec)が来てくれることを信じて待つ
                        
                    */
                }
            }
        }

    }
}
