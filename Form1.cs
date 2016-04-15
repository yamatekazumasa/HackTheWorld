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
            pressedKeys = new LinkedList<Keys>();
            GraphicsContext = Graphics.FromImage(_bmp);
            Scene.Current = new TitleScene();

            while (!IsDisposed) // 毎フレーム呼ばれる処理
            {

                Input.Update(pressedKeys);

                // プレイヤーとステージをアップデート
                Scene.Current.Update();

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


    }
}
