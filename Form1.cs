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
using InoueLab;

namespace HackTheWorld
{
    public partial class Form1 : Form
    {
        private Bitmap _bmp;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            ThreadSeparate(ref _drawThread, MainProcess);
        }

        bool Dragging=false;
    
        List<Point> _MouseLocations = new List<Point>();
        public Point[] MouseLovations = new Point[0];
        /// <summary>
        /// マウスの左が押させた場合Draggngをtrueにする
        /// マウスの座標をMouselocationsに保存する
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            Cursor.Current = Cursors.Hand;
            Dragging = true;
            _MouseLocations.Add(e.Location);
        }
        /// <summary>
        /// ドラッグが終了した際Draggingをfalseに戻す
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            Dragging = false;
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
        }
        

        private void MainProcess()
        {
            _bmp = new Bitmap(ScreenWidth, ScreenHeight);
            GraphicsContext = Graphics.FromImage(_bmp);
            Scene.Current = new TitleScene();
            int frame = 0;

            while (!IsDisposed) // 毎フレーム呼ばれる処理
            {
                frame++;

                if (frame == 300)
                {
                    Scene.Push(new GameScene());
                }

                if (frame == 500)
                {
                    Scene.Pop();
                }

                // プレイヤーとステージをアップデート
                Scene.Current.Update();
 //               if (Dragging) GraphicsContext.DrawEllipse(Pens.Aqua, 0, 0,10,10);
                // 画面の更新
                InterThreadRefresh(Refresh);

            }

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
