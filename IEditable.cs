using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static HackTheWorld.Constants;
using HackTheWorld;

namespace HackTheWorld
{
    /// <summary>
    /// 編集可能インターフェース。
    /// CodeBox で編集するオブジェクトに継承させる。
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public interface IEditable
    {
        /// <summary>
        /// 何番目の Process が実行されているか。
        /// </summary>
        int ProcessPtr { get; set; }
        /// <summary>
        /// 自身のコードを編集するテキストエディタ。
        /// </summary>
        CodeBox Codebox { get; set; }
        /// <summary>
        /// 自身の動作を格納する。
        /// </summary>
        List<Process> Processes { get; set; }
        /// <summary>
        /// true のとき Update() で Process が実行されるようになる。
        /// </summary>
        bool CanExecute { get; set; }
        // GameObject 由来のプロパティ
        float X { get; set; }
        float Y { get; set; }
        float VX { get; set; }
        float VY { get; set; }
        float W { get; set; }
        float H { get; set; }
        float MinX { get; set; }
        float MinY { get; set; }
        float MidX { get; set; }
        float MidY { get; set; }
        float MaxX { get; set; }
        float MaxY { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        Vector Position { get; set; }
        Vector Velocity { get; set; }
        Vector Size { get; set; }
        bool Clicked { get; }
        bool IsAlive { get; }
        void Move(float dt);
        void Rotate(double degree);
        void Accelerate(double a);
        bool Contains(Point p);
        bool Contains(GameObject obj);
        bool Intersects(GameObject obj);
        bool CollidesWith(GameObject obj);
        bool RiddenBy(GameObject obj);
        bool Nearby(GameObject obj);
        bool InWindow();
    }

    static partial class Extensions
    {
        public static void Focus(this IEditable self)
        {
            self.Codebox.Focus();
        }

        public static bool IsFocused(this IEditable self)
        {
            return self.Codebox.IsFocused;
        }

        public static void SetProcesses(this IEditable self, Process[] processes)
        {
            self.Processes = processes.ToList();
        }

        public static void SetProcesses(this IEditable self, List<Process> processes)
        {
            self.Processes = processes;
        }

        public static void AddProcess(this IEditable self, Process process)
        {
            if (self.Processes == null) self.Processes = new List<Process>();
            self.Processes.Add(process);
        }

        public static void AddProcess(this IEditable self, ExecuteWith executeWith, float seconds)
        {
            if (self.Processes == null) self.Processes = new List<Process>();
            self.Processes.Add(new Process(executeWith, seconds));
        }

        public static void AddProcess(this IEditable self, ExecuteWith executeWith)
        {
            if (self.Processes == null) self.Processes = new List<Process>();
            self.Processes.Add(new Process(executeWith));
        }

        public static void Update(this IEditable self, float dt)
        {
            if (Scene.Current is EditScene)
            {
                if (self.Clicked) self.Codebox.Focus();
                self.Codebox.Update();
            }
            if (!self.CanExecute || self.Processes == null) return;

            var process = self.Processes[self.ProcessPtr];
            if (process.ElapsedTime*1000 <= process.MilliSeconds)
            {
                process.ExecuteWith(self, dt);
                process.ElapsedTime += dt;
            }
            else if(self.ProcessPtr + 1 < self.Processes.Count)
            {
                self.ProcessPtr++;
            }
        }

        public static void Compile(this IEditable self, Stage stage)
        {
            string str = self.Codebox.GetString();
            // ここにstring型をProcess型に変換する処理を書く。
            // self.SetProcesses(new Process[] {});
        }

        public static void SetDemoProcesses(this IEditable self, Stage s)
        {
            for (int i=0; i<100; i++)
            {
                if(i%2==0) self.AddProcess((obj, dt) => { obj.VX = CellSize; });
                else       self.AddProcess((obj, dt) => { obj.VX = -CellSize; });
                self.AddProcess((obj, dt) => { obj.Move(dt); }, 2.0f);
                self.AddProcess((obj, dt) => { obj.VX = 0; });
                self.AddProcess((obj, dt) => { obj.Move(dt); }, 0.25f);
                self.AddProcess((obj, dt) => {
                    if (obj.Nearby(s.Player)) {
                        var b = new Bullet(self.X, self.MidY, -50, 0, 10, 10);
                        s.Bullets.Add(b);
                        s.Objects.Add(b);
                    }
                });
                self.AddProcess((obj, dt) => { obj.Move(dt); }, 0.25f);
            }

        }

        public static void Execute(this IEditable self)
        {
            self.ProcessPtr = 0;
            self.CanExecute = true;
        }


    }

}
