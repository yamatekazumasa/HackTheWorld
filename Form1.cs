using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;



namespace HackTheWorld
{
    /// <summary>
    /// ゲームウィンドウ。
    /// キー入力やマウス入力、画面への出力などユーザーとのインタラクション全般を担う。
    /// </summary>
    public partial class Form1 : Form
    {
        private Bitmap _bmp;
        private LinkedList<Keys> _pressedKeys;
        private LinkedList<MouseButtons> _pressedButtons;

        public Form1()
        {
            InitializeComponent();
   
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;//サイズの固定
            this.MaximizeBox = false;

            WindowContext = this;

            Shown += (sender, e) => { Task.Run(() => { MainProcess(); }); };

        }

        private void MainProcess()
        {
            _bmp = new Bitmap(ScreenWidth, ScreenHeight);
            GraphicsContext = Graphics.FromImage(_bmp);

            _pressedKeys = new LinkedList<Keys>();
            _pressedButtons = new LinkedList<MouseButtons>();
            
            Scene.Current = new TitleScene();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long prevTime = stopwatch.ElapsedMilliseconds;

            while (!IsDisposed) // 毎フレーム呼ばれる処理
            {
                long currentTime = stopwatch.ElapsedMilliseconds;
                if (currentTime > 100000000000000000) stopwatch.Restart();
                float dt = (currentTime - prevTime) / 1000.0F;

                Input.Update(_pressedKeys);
                Input.Mouse.Update(_pressedButtons);
                Input.Mouse.Update(MousePosition, Location);
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

//                if (new Rectangle(0, 0, 500, 500).Contains(Input.Mouse.Position))
//                {
//                    Invoke((Action)(() => {
//                        System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.IBeam;
//                    }));
//                }

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

        /// <summary>
        /// 文字入力取得用。
        /// 押された文字（キーではない）を格納する。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // バックスペース(\n)、SOF(\u0001)、改行(\r,\n)、タブ(\t)は除外。
            if (e.KeyChar == '\b' || e.KeyChar == '\u0001' || e.KeyChar == '\r' || e.KeyChar == '\n' || e.KeyChar == '\t') return;
            Input.KeyBoard.Append(e.KeyChar);
        }

        /// <summary>
        /// マウスのボタン入力取得用。
        /// 押されたボタンの状態を更新する。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_pressedButtons.Contains(e.Button)) _pressedButtons.AddLast(e.Button);
        }

        /// <summary>
        /// マウスのボタン入力取得用。
        /// 離されたボタンの状態を更新する。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pressedButtons.Remove(e.Button);
        }

        /// <summary>
        /// フォームの描画時に _bmp を描画する。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_bmp == null) return;
            e.Graphics.DrawImage(_bmp, 0, 0);
        }

    }
}
