using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;



namespace HackTheWorld
{

    public partial class Form1 : Form
    {
        private Bitmap _bmp;
        private LinkedList<Keys> _pressedKeys;
        private LinkedList<MouseButtons> _mouseButtons;

        public Form1()
        {
            InitializeComponent();
   
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;//サイズの固定
            this.MaximizeBox = false;

            Shown += (sender, e) => { Task.Run(() => { MainProcess(); }); };

        }

        //なおせない
        private void MainProcess()
        {
            _bmp = new Bitmap(ScreenWidth, ScreenHeight);

            _pressedKeys = new LinkedList<Keys>();
            _mouseButtons = new LinkedList<MouseButtons>();

            Invoke((Action)(() => { GraphicsContext = Graphics.FromImage(_bmp); }));
            
            Scene.Current = new TitleScene();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long prevTime = stopwatch.ElapsedMilliseconds;

            while (!IsDisposed) // 毎フレーム呼ばれる処理
            {
                long currentTime = stopwatch.ElapsedMilliseconds;
                if (currentTime > 100000) stopwatch.Restart();
                float dt = (currentTime - prevTime) / 1000.0F;

                Input.Update(_pressedKeys);
                Input.Update(_mouseButtons);
                Input.Update(MousePosition, Location);

                // プレイヤーとステージをアップデート
                Scene.Current.Update(dt);

#if DEBUG
                // デバッグ用文字列
                string debugDt = "dt:  " + ((int)(dt*1000)).ToString("D4") + "[ms]";
                string debugFps = "FPS: " + ((int)(1000 / dt)).ToString("D6");
                string debugSeconds = "sec: " + (currentTime/1000f).ToString("F1") + "[s]";
                Font font = new Font("Courier New", 12);
                GraphicsContext.DrawString(debugDt, font, Brushes.Black, ScreenWidth - 140, 0);
                GraphicsContext.DrawString(debugFps, font, Brushes.Black, ScreenWidth - 140, 20);
                GraphicsContext.DrawString(debugSeconds, font, Brushes.Black, ScreenWidth - 140, 40);
#endif

                // 画面の更新
                if (InvokeRequired)
                    try { Invoke((Action)Refresh); }
                    catch (Exception) { }
                else Refresh();

                prevTime = currentTime;

            }

        }

        /// <summary>
        /// キー入力取得用。
        /// 押されたキーをpressedKeysに格納する。
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.KeyCode)) _pressedKeys.AddLast(e.KeyCode);
        }

        /// <summary>
        /// キー入力取得用。
        /// キーが離されるとpressedKeysから除外する。
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // バックスペース(\n)、SOF(\u0001)、改行(\r,\n)、タブ(\t)は除外。
            if (e.KeyChar == '\b' || e.KeyChar == '\u0001' || e.KeyChar == '\r' || e.KeyChar == '\n' || e.KeyChar == '\t') return;
            Input.KeyBoard.Append(e.KeyChar);
        }

        //押されているマウスのボタン
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_mouseButtons.Contains(e.Button)) _mouseButtons.AddLast(e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseButtons.Remove(e.Button);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (_bmp == null) return;
            e.Graphics.DrawImage(_bmp, 0, 0);
        }

<<<<<<< HEAD
=======
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

        
>>>>>>> 2195bc0b33123fb1617b8c007328dcae5df5c5a4
    }
}
