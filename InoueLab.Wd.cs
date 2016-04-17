using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using System.Windows.Forms.DataVisualization.Charting;
//Formアプリケーションで使用可能．Consoleアプリケーションで使用不可

namespace InoueLab
{
    #region Int2, Int3, Double2, Double3
    public partial struct Int2 : IEquatable<Int2>, IComparable<Int2>
    {
        public static explicit operator Point(Int2 value) { return new Point(value.v0, value.v1); }
        public static explicit operator Size(Int2 value) { return new Size(value.v0, value.v1); }
        public static explicit operator Rectangle(Int2 value) { return new Rectangle(0, 0, value.v0, value.v1); }
        public static explicit operator PointF(Int2 value) { return new PointF(value.v0, value.v1); }
        public static explicit operator SizeF(Int2 value) { return new SizeF(value.v0, value.v1); }
        public static explicit operator RectangleF(Int2 value) { return new RectangleF(0, 0, value.v0, value.v1); }
    }

    public partial struct Double2 : IEquatable<Double2>, IComparable<Double2>
    {
        public static explicit operator Point(Double2 value) { return new Point((int)value.v0, (int)value.v1); }
        public static explicit operator Size(Double2 value) { return new Size((int)value.v0, (int)value.v1); }
        public static explicit operator Rectangle(Double2 value) { return new Rectangle(0, 0, (int)value.v0, (int)value.v1); }
        public static explicit operator PointF(Double2 value) { return new PointF((float)value.v0, (float)value.v1); }
        public static explicit operator SizeF(Double2 value) { return new SizeF((float)value.v0, (float)value.v1); }
        public static explicit operator RectangleF(Double2 value) { return new RectangleF(0, 0, (float)value.v0, (float)value.v1); }
    }
    #endregion

    #region ConsoleWindow
    public class ConsoleWindow : TextWriter
    {
        Form form;
        ListBox console;
        Action function;
        string bottomLine = null;
        public ConsoleWindow(Form form, ListBox console, Action function = null)
        {
            if (form == null) ThrowException.ArgumentNull("form");
            if (console == null) ThrowException.ArgumentNull("console");
            this.form = form;
            this.console = console;
            this.function = function;
        }
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Unicode; }
        }

        void __Write(string value, bool newline)
        {
            console.BeginUpdate();
            var items = console.Items;
            if (value.Length > 0)
            {
                string[] lines = value.Split(Ct.NewLineCodes, StringSplitOptions.None);
                if (bottomLine == null) items.Add(lines[0]);
                else items[items.Count - 1] = bottomLine + lines[0];
                for (int i = 0; ++i < lines.Length;) items.Add(lines[i]);
                bottomLine = (string)items[items.Count - 1];
            }
            if (newline)
            {
                if (bottomLine == null) items.Add("");
                bottomLine = null;
            }
            //console.SetSelected(items.Count - 1, true);
            console.TopIndex = items.Count - 1;
            console.EndUpdate();
            console.Refresh();
        }
        void _Write(string value, bool newline)
        {
            if (function != null) function();
            form.InvokeIfRequired(() => __Write(value, newline));
        }
        public override void Write(string value) { _Write(value, false); }
        public override void WriteLine(string value) { _Write(value, true); }
        public override void WriteLine() { _Write("", true); }

        //デフォルトでは改行が別に行われるため遅い．一緒にやることで高速化
        public override void WriteLine(bool value) { WriteLine(value.ToString()); }
        public override void WriteLine(char value) { WriteLine(value.ToString()); }
        //public override void WriteLine(char[] buffer) { WriteLine(new string(buffer)); }
        //public override void WriteLine(char[] buffer, int index, int count) { WriteLine(new string(buffer, index, count)); }
        public override void WriteLine(decimal value) { WriteLine(value.ToString()); }
        public override void WriteLine(double value) { WriteLine(value.ToString()); }
        public override void WriteLine(float value) { WriteLine(value.ToString()); }
        public override void WriteLine(int value) { WriteLine(value.ToString()); }
        public override void WriteLine(long value) { WriteLine(value.ToString()); }
        public override void WriteLine(object value) { WriteLine(value.ToString()); }
        public override void WriteLine(string format, object arg0) { WriteLine(string.Format(format, arg0)); }
        public override void WriteLine(string format, object arg0, object arg1) { WriteLine(string.Format(format, arg0, arg1)); }
        public override void WriteLine(string format, object arg0, object arg1, object arg2) { WriteLine(string.Format(format, arg0, arg1, arg2)); }
        public override void WriteLine(string format, params object[] arg) { WriteLine(string.Format(format, arg)); }
        public override void WriteLine(uint value) { WriteLine(value.ToString()); }
        public override void WriteLine(ulong value) { WriteLine(value.ToString()); }
    }
    #endregion

    #region Custodian
    public class AutoPressAcceptButton
    {
        Form form;
        public AutoPressAcceptButton(Form form)
        {
            this.form = form;
            Application.Idle += ApplicationIdle;
        }
        void ApplicationIdle(object sender, EventArgs e)
        {
            Application.Idle -= ApplicationIdle;
            form.AcceptButton.PerformClick();
        }
    }
    public static class ApplicationIdle
    {
        static List<Action> Actions = new List<Action>();
        public static void Add(Action action)
        {
            Actions.Add(action);
            Application.Idle += ApplicationIdleEvent;
        }
        static void ApplicationIdleEvent(object sender, EventArgs e)
        {
            Application.Idle -= ApplicationIdleEvent;
            foreach (var action in Actions) action();
        }
    }
    public class Custodian
    {
        static readonly Type[] Types = { 
            typeof(TextBox), typeof(RadioButton), typeof(CheckBox), typeof(ToolStripTextBox), typeof(TabControl)
        };
        static int TargetID(object obj) { return Array.IndexOf(Types, obj.GetType()); }
        static string TargetName(object obj)
        {
            var toolStripItem = obj as ToolStripItem;
            return toolStripItem != null ? toolStripItem.Name : ((Control)obj).Name;
        }

        List<object> Items = new List<object>();
        string fileName;
        public Custodian(Form form, string fileName)
        {
            this.fileName = fileName;
            Custodian_(form);
        }
        public Custodian(Form form)
        {
            this.fileName = Path.ChangeExtension(Application.ExecutablePath, "ini");
            Custodian_(form);
        }

        void Custodian_(Form form)
        {
            Listup(form);
            Read();
            form.FormClosing += (sender, e) => { Write(); };
            if (form.AcceptButton != null)
            {
                CheckBox c = Items.FirstOrDefault(i => TargetName(i).Contains("AutoPressAcceptButton")) as CheckBox;
                if (c != null && c.Checked)
                    ApplicationIdle.Add(() => { form.AcceptButton.PerformClick(); });
            }
        }
        void Listup(Control control)
        {
            var toolStrip = control as ToolStrip;
            if (toolStrip != null)
            {
                foreach (ToolStripItem t in toolStrip.Items)
                {
                    if (TargetID(t) >= 0) Items.Add(t);
                }
            }
            else
            {
                foreach (Control c in control.Controls)
                {
                    if (TargetID(c) >= 0) Items.Add(c);
                    Listup(c);
                }
            }
        }

        public void Write()
        {
            using (var file = new StreamWriter(fileName))
            {
                foreach (object o in Items)
                {
                    string s = TargetName(o) + "\t";
                    switch (TargetID(o))
                    {
                        case 0: s += ((TextBox)o).Text; break;
                        case 1: s += ((RadioButton)o).Checked; break;
                        case 2: s += ((CheckBox)o).Checked; break;
                        case 3: s += ((ToolStripTextBox)o).Text; break;
                        case 4: s += ((TabControl)o).SelectedIndex; break;
                    }
                    file.WriteLine(s);
                }
            }
        }
        public void Read()
        {
            if (!File.Exists(fileName)) return;
            using (var file = new StreamReader(fileName))
            {
                while (!file.EndOfStream)
                {
                    string[] s = file.ReadLine().Split('\t');
                    if (s.Length != 2) break;
                    object o = Items.FirstOrDefault(i => TargetName(i) == s[0]);
                    if (o == null) continue;

                    switch (TargetID(o))
                    {
                        case 0: ((TextBox)o).Text = s[1]; break;
                        case 1: ((RadioButton)o).Checked = bool.Parse(s[1]); break;
                        case 2: ((CheckBox)o).Checked = bool.Parse(s[1]); break;
                        case 3: ((ToolStripTextBox)o).Text = s[1]; break;
                        case 4: ((TabControl)o).SelectedIndex = int.Parse(s[1]); break;
                    }
                }
            }
        }
    }
    #endregion

    #region FileDropped
    public class FileDropped
    {
        Action<string[]> FileDroppedFunc;
        public FileDropped(Form form, Action<string[]> func)
        {
            FileDroppedFunc = func;
            form.DragEnter += DragEnter;
            form.DragDrop += DragDrop;
            form.AllowDrop = true;
        }
        void DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        void DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;
            FileDroppedFunc(files);
        }
    }
    #endregion

    // Extended constructors
    public static partial class New
    {
        #region Image
        public static PointF PointF(double x, double y) { return new PointF((float)x, (float)y); }
        public static SizeF SizeF(double width, double height) { return new SizeF((float)width, (float)height); }
        public static RectangleF RectangleF(double x, double y, double width, double height) { return new RectangleF((float)x, (float)y, (float)width, (float)height); }
        static int UnitTo256(double x) { return (int)Mt.MinMax(x * 256, 0, 255); }
        public static Color Color(double a, double r, double g, double b) { return System.Drawing.Color.FromArgb(UnitTo256(a), UnitTo256(r), UnitTo256(g), UnitTo256(b)); }
        public static Color Color(double r, double g, double b) { return System.Drawing.Color.FromArgb(UnitTo256(r), UnitTo256(g), UnitTo256(b)); }

        public static ColorMap ColorMap(Color oldColor, Color newColor)
        {
            ColorMap m = new ColorMap();
            m.OldColor = oldColor;
            m.NewColor = newColor;
            return m;
        }
        public static ImageAttributes ImageAttributes(ColorMap[] map)
        {
            ImageAttributes a = new ImageAttributes();
            a.SetRemapTable(map);
            return a;
        }
        public static ImageAttributes ImageAttributes(ColorMatrix cm)
        {
            ImageAttributes a = new ImageAttributes();
            a.SetColorMatrix(cm);
            return a;
        }

        static readonly ContentAlignment[] ContentAlignments = new ContentAlignment[]{
            ContentAlignment.TopLeft, ContentAlignment.TopCenter, ContentAlignment.TopRight,
            ContentAlignment.MiddleLeft, ContentAlignment.MiddleCenter, ContentAlignment.MiddleRight,
            ContentAlignment.BottomLeft, ContentAlignment.BottomCenter, ContentAlignment.BottomRight
        };
        static readonly StringAlignment[] StringAlignments = new StringAlignment[]{
            StringAlignment.Near, StringAlignment.Center, StringAlignment.Far
        };
        public static StringFormat StringFormat(ContentAlignment alignment)
        {
            StringFormat format = new StringFormat();
            int i = ContentAlignments.IndexOf(alignment);
            format.LineAlignment = StringAlignments[i / 3];
            format.Alignment = StringAlignments[i % 3];
            return format;
        }

        public static Bitmap Bitmap(Size size, PixelFormat format = PixelFormat.Format24bppRgb)
        {
            return new Bitmap(size.Width, size.Height, format);
        }
        // 画像ファイルを読み込んだ後ロックを解除する
        public static Bitmap Bitmap(string path)
        {
            using (var bitmap = new Bitmap(path))
            {
                var result = New.Bitmap(bitmap.Size);
                Graphics.FromImage(result).DrawImage(bitmap, 0, 0);
                return result;
            }
        }
        #endregion
    }
    // Utility and extension methods
    public static partial class Ex
    {
        #region Image
        public static void Let<TImage>(ref TImage oldImage, TImage newImage) where TImage : Image
        {
            if (oldImage != null) oldImage.Dispose();
            oldImage = newImage;
        }
        public static BitmapData LockBits(this Bitmap bitmap)
        {
            return bitmap.LockBits(new Rectangle(default(Point), bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        }

        public static void DrawImage(this Graphics graphics, Image image, Point destPoint)
        {
            graphics.DrawImage(image, destPoint.X, destPoint.Y);
        }
        public static void DrawImage(this Graphics graphics, Image image, Point destPoint, ImageAttributes imageAttrs)
        {
            graphics.DrawImage(image, new Rectangle(destPoint, image.Size), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttrs);
        }
        public static void DrawImage(this Graphics graphics, Image image, Point destPoint, Rectangle srcRect)
        {
            graphics.DrawImage(image, destPoint.X, destPoint.Y, srcRect, GraphicsUnit.Pixel);
        }
        public static void DrawImage(this Graphics graphics, Image image, Point destPoint, Rectangle srcRect, ImageAttributes imageAttrs)
        {
            graphics.DrawImage(image, new Rectangle(destPoint, srcRect.Size), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttrs);
        }

        public static Bitmap CloneX(this Bitmap bitmap)
        {
            return (Bitmap)bitmap.Clone();
        }
        public static Bitmap CloneX(this Bitmap bitmap, Rectangle rectangle)
        {
            return bitmap.Clone(rectangle, bitmap.PixelFormat);
        }
        public static Bitmap CloneX(this Bitmap bitmap, RectangleF rectangle)
        {
            return bitmap.Clone(rectangle, bitmap.PixelFormat);
        }
        #endregion

        #region Control
        public static void Invoke(this Control control, Action method)
        {
            control.Invoke(method);
        }
        public static T Invoke<T>(this Control control, Func<T> method)
        {
            return (T)control.Invoke(method);
        }
        public static void InvokeIfRequired(this Control control, Action method)
        {
            if (control.InvokeRequired)
                try { control.Invoke(method); }
                catch (Exception) { }
            else method();
        }
        public static T InvokeIfRequired<T>(this Control control, Func<T> method)
        {
            if (control.InvokeRequired) return (T)control.Invoke(method);
            else return method();
        }

        public static void ForEachDownward(this Control control, Action<Control> method)
        {
            method(control);
            foreach (Control item in control.Controls)
                item.ForEachDownward(method);
        }
        public static void ForEachUpward(this Control control, Action<Control> method)
        {
            foreach (Control item in control.Controls)
                item.ForEachUpward(method);
            method(control);
        }
        public static void ForEachChildDownward(this Control control, Action<Control> method)
        {
            foreach (Control item in control.Controls)
            {
                method(control);
                item.ForEachChildDownward(method);
            }
        }
        public static void ForEachChildUpward(this Control control, Action<Control> method)
        {
            foreach (Control item in control.Controls)
            {
                item.ForEachChildUpward(method);
                method(control);
            }
        }
        #endregion

    //    #region Chart
    //    public static void InitializeLegends(this Chart chart, string[] legends)
    //    {
    //        chart.InvokeIfRequired(() =>
    //        {
    //            chart.Series.Clear();
    //            foreach (var legend in legends)
    //                chart.Series.Add(legend);
    //            foreach (var series in chart.Series)
    //                series.ChartType = SeriesChartType.FastPoint;
    //        });
    //    }
    //    public static void InitializeAxes(this Chart chart, Double2 rangeX, Double2 rangeY)
    //    {
    //        chart.InvokeIfRequired(() =>
    //        {
    //            if (!rangeX.v0.IsNaN()) chart.ChartAreas[0].AxisX.Minimum = rangeX.v0;
    //            if (!rangeX.v1.IsNaN()) chart.ChartAreas[0].AxisX.Maximum = rangeX.v1;
    //            if (!rangeY.v0.IsNaN()) chart.ChartAreas[0].AxisY.Minimum = rangeY.v0;
    //            if (!rangeY.v1.IsNaN()) chart.ChartAreas[0].AxisY.Maximum = rangeY.v1;
    //        });
    //    }
    //    public static void ClearXY(this Chart chart)
    //    {
    //        chart.InvokeIfRequired(() =>
    //        {
    //            foreach (var series in chart.Series)
    //                series.Points.Clear();
    //        });
    //    }
    //    public static void AddXY(this Chart chart, double x, double[] y)
    //    {
    //        chart.InvokeIfRequired(() =>
    //        {
    //            chart.Series.ForEach((series, i) =>
    //            {
    //                series.Points.AddXY(x, y[i]);
    //            });
    //        });
    //    }
    //    public static void AddXY(this Chart chart, double[] x, double[][] y)
    //    {
    //        chart.InvokeIfRequired(() =>
    //        {
    //            chart.Series.ForEach((series, i) =>
    //            {
    //                var yi = y[i];
    //                for (int j = 0; j < x.Length; j++)
    //                    series.Points.AddXY(x[j], yi[j]);
    //            });
    //        });
    //    }
    //    public static void SetXY(this Chart chart, double[] x, double[][] y)
    //    {
    //        chart.InvokeIfRequired(() =>
    //        {
    //            chart.ClearXY();
    //            chart.AddXY(x, y);
    //        });
    //    }
    //    #endregion
    }

    // Human-based functions
    public static partial class Hm
    {
        #region Image functions
        static Color CreateColor_(double hue, double a, double b)
        {
            return New.Color(
                b + a * Math.Cos(hue),
                b + a * Math.Cos(hue - Mt.PI2 / 3),
                b + a * Math.Cos(hue + Mt.PI2 / 3)
            );
        }
        // hue: radian, 0<=saturation<=1, 0<=lightness<=1
        public static Color ColorFromHSL(double hue, double saturation, double lightness)
        {
            return CreateColor_(hue, 0.5 * saturation * (1 - Math.Abs(lightness * 2 - 1)), lightness);
        }
        // hue: radian, 0<=saturation<=1, 0<=value<=1
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            return CreateColor_(hue, 0.5 * value * saturation, 0.5 * value * (2 - saturation));
        }
        public static Color ColorFromV(double value)
        {
            var v = (int)Mt.MinMax(value * 0x100, 0, 0xff);
            return Color.FromArgb(v, v, v);
        }

        public static Bitmap CreateBitmap(Size size, Func<int, int, Color> func)
        {
            var bitmap = New.Bitmap(size, PixelFormat.Format32bppArgb);
            var lockdata = bitmap.LockBits();
            for (int y = 0; y < size.Height; y++)
            {
                IntPtr p = lockdata.Scan0 + y * lockdata.Stride;
                for (int x = 0; x < size.Width; x++)
                    unsafe { ((int*)p)[x] = func(x, y).ToArgb(); }
            }
            bitmap.UnlockBits(lockdata);
            return bitmap;
        }
        public static Bitmap CreateBitmap(Color[,] image)
        {
            return CreateBitmap(new Size(image.GetLength(1), image.GetLength(0)), (x, y) => image[y, x]);
        }

        public static double[,] NormalizeImage(double[,] image, Double2 range)
        {
            if (range == default(Double2))
            {
                var m = image.ToEnumerable().MinMaxItem();
                range = m.v0 == m.v1 ? new Double2(m.v0 - 0.5, m.v1 + 0.5) : new Double2(m.v0, m.v1);
            }
            var scale = 1 / (range.v1 - range.v0);
            return image.SelectTo(v => (v - range.v0) * scale);
        }
        public static complex[,] NormalizeImage(complex[,] image, Double2 range)
        {
            if (range == default(Double2))
            {
                var m = image.ToEnumerable().Select(v => v.Magnitude).MinMaxItem();
                range = m.v0 == m.v1 ? new Double2(m.v0 - 0.5, m.v1 + 0.5) : new Double2(m.v0, m.v1);
            }
            var scale = 1 / (range.v1 - range.v0);
            return image.SelectTo(v => v * ((1 - range.v0 / v.Magnitude) * scale));
        }

        public static Bitmap CreateBitmap(double[,] image, Double2 range) { return CreateBitmap(NormalizeImage(image, range)); }
        public static Bitmap CreateBitmap(double[,] image)
        {
            return CreateBitmap(new Size(image.GetLength(1), image.GetLength(0)), (x, y) =>
            {
                var v = Mt.MinMax(image[y, x], 0, 1);
                return Hm.ColorFromV(v);
            });
        }
        public static Bitmap CreateBitmap(complex[,] image, Double2 range) { return CreateBitmap(NormalizeImage(image, range)); }
        public static Bitmap CreateBitmap(complex[,] image)
        {
            return CreateBitmap(new Size(image.GetLength(1), image.GetLength(0)), (x, y) =>
            {
                var v = Mt.MinMax(image[y, x].Magnitude, 0, 1);
                var h = image[y, x].Phase;
                return Hm.ColorFromHSV(h, 1, v);
            });
        }

        // .NET Framework, System.Drawing.dll のバグにより PixelFormat.Format16bppGrayScale は使用できない
        // BitmapSource, System.Windows.Media.PixelFormats.Gray16 を使えば作成可能
        public static Bitmap CreateBitmapGray16(double[,] image, Double2 range) { return CreateBitmapGray16(NormalizeImage(image, range)); }
        public static Bitmap CreateBitmapGray16(double[,] image)
        {
            var size = new Size(image.GetLength(1), image.GetLength(0));
            var bitmap = New.Bitmap(size, PixelFormat.Format64bppArgb);
            var lockdata = bitmap.LockBits();
            for (int y = 0; y < size.Height; y++)
            {
                IntPtr p = lockdata.Scan0 + y * lockdata.Stride;
                for (int x = 0; x < size.Width; x++)
                {
                    var v = (uint)Mt.MinMax(image[y, x] * 0x2000, 0, 0x1fff);
                    unsafe { ((ulong*)p)[x] = v * 0x000100010001ul + 0x1fff000000000000ul; }  //薄くなる
                }
            }
            bitmap.UnlockBits(lockdata);
            return bitmap;
        }

        public sealed class Pane : IDisposable
        {
            Graphics graphics;
            RectangleF rect;
            Region clip;
            public Pane(Graphics graphics, RectangleF rect)
            {
                this.graphics = graphics;
                this.rect = rect;
                clip = graphics.Clip;
                graphics.IntersectClip(rect);
                graphics.TranslateTransform(rect.Left, rect.Top);
            }
            #region IDisposable メンバー
            public void Dispose()
            {
                graphics.TranslateTransform(-rect.Left, -rect.Top);
                graphics.Clip = clip;
            }
            #endregion
        }
        #endregion
    }
}