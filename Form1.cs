using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;
using static HackTheWorld.Input;



namespace HackTheWorld

    
{



    public partial class Form1 : Form
    {
        private Bitmap _bmp;
        private List<MouseButtons> mouseButtons;

        private LinkedList<Keys> pressedKeys; 

        public Form1()
        {
            InitializeComponent();
   
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            ThreadSeparate(ref _drawThread, MainProcess);


        }


        private void MainProcess()
        {
            _bmp = new Bitmap(ScreenWidth, ScreenHeight);

            mouseButtons = new List<MouseButtons>();

            pressedKeys = new LinkedList<Keys>();
            GraphicsContext = Graphics.FromImage(_bmp);
            Scene.Current = new TitleScene();
            while (!IsDisposed) // 毎フレーム呼ばれる処理
            {

                Input.Update(pressedKeys);
                Input.Update(mouseButtons);
                Input.Update(this.Location, MousePosition);
                // プレイヤーとステージをアップデート
                Scene.Current.Update();
               
                //if (Dragging) GraphicsContext.DrawEllipse(Pens.Aqua, 0, 0,10,10);
                // 画面の更新
                InterThreadRefresh(Refresh);

            }

        }




        /// <summary>
        /// キー入力取得用。
        /// 押されたキーをpressedKeysに格納する。
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!pressedKeys.Contains(e.KeyCode)) pressedKeys.AddLast(e.KeyCode);
            Console.WriteLine(String.Join(",", pressedKeys));
        }
        /// <summary>
        /// キー入力取得用。
        /// キーが離されるとpressedKeysから除外する。
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
            Console.WriteLine(String.Join(",", pressedKeys));
        }

        //押されているマウスのボタン

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!mouseButtons.Contains(e.Button)) mouseButtons.Add(e.Button);
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseButtons.Remove(e.Button);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_bmp, 0, 0);
        }

        private Thread _drawThread;

        private void ThreadSeparate(ref Thread _thread, Action _function)
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
            }
            _thread = new Thread(new ThreadStart(_function));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void InterThreadRefresh(Action _function)
        {
            try
            {
                if (InvokeRequired) Invoke(_function);
                else _function();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        //ここからいじった
        private void textBox1_TextChanged(object sender , EventArgs e)
        {
            
        }
        private void button1_Click(object sender , EventArgs e)
        {
            textBox2.Text = "";
            For( );
            FourOperations( );
        }
        //private void Wait( )
        //{
        //      string test = textBox1.Text;
        //    string [] testdata =test.Split(' ');
        //}
        //的な感じで他のも作りたい
        private void FourOperations()
        {
            //とりあえず数字の計算をさせたい
            //textbox1の内容を計算してtextbox2に出す
            string test = textBox1.Text;
            string[ ] testdata = test.Split(' ');

            //For()のほうでやらせたいやつだったら無視
            if(testdata[0] == "for")
            {
                return;
            }

            //四則演算の式になっていないとうまく使えないので書いている途中は何もしない
            if(test.EndsWith("+") || test.EndsWith("-") || test.EndsWith("*") || test.EndsWith("/") || test.EndsWith("."))
            {
                return;
            }

            //ここで計算
            System.Data.DataTable dt = new System.Data.DataTable( );


            //出力するとき型があってないといけないらしいので型をとって条件分岐
            Type t = dt.Compute(test , "").GetType( );

            //型の確かめ用
            //textBox2.Text = t.ToString( );

            //分岐
            if(t.ToString( ) == "System.DBNull")
            {
                textBox2.Text = "なんもない";
                listBox1.Items.Add("ばなな");

                //listboxから要素をとってくるテスト
                //選択→選択したアイテムを使う
                listBox1.SetSelected(0 , true);
                textBox2.Text = listBox1.SelectedItems[0].ToString( );
            }
            else {
                if(t.ToString( ) == "System.Int32")
                {
                    int result = (int)dt.Compute(test , "");
                    textBox2.Text = result.ToString( );
                }
                else
                {
                    double result = (double)dt.Compute(test , "");
                    textBox2.Text = result.ToString( );
                }
            }
        }
        private void For( )
        {
            //入力 半角の空白ごとに取得したい
            //for 3 jump() とかを想定
            string test = textBox1.Text;
            string[ ] testdata = test.Split(' ');

            //この形になってなかったら無視
            if(testdata.Length < 3)
            {
                return;
            }
            else {
                if(testdata[0] == "for")
                {
                    //testdata[1] = FourOperations(testdata[1]);
                    int num = int.Parse(testdata[1]);
                    for(int i = 0; i < num; i++)
                    {
                        for(int j = 2; j < testdata.Length; j++)
                        {
                            if(i == num - 1)
                            {
                                textBox2.Text += testdata[j];
                            }
                            else {
                                textBox2.Text += testdata[j] + " ";
                            }
                        }
                    }
                }
            }
        }

        
    }
}
