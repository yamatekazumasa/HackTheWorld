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
        int ProcessPtr { get; set; }
        IEnumerator Routine { get; set; }
        CodeBox Codebox { get; }
        List<Process> Processes { get; set; }
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

        public static bool CanExecute(this IEditable self)
        {
            return self.Routine != null;
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
            self.Processes.Add(process);
        }

        public static void Update(this IEditable self, float dt)
        {
            if (self.Processes == null) return;
            var process = self.Processes[self.ProcessPtr];
            if (process.ElapsedTime * 1000 <= process.MilliSeconds)
            {
                process.ExecuteWith(self, dt);
                process.ElapsedTime += dt;
            }
            else if (self.ProcessPtr + 1 < self.Processes.Count)
            {
                self.ProcessPtr++;
            }

            if (self.Clicked) self.Codebox.Focus();
            self.Codebox.Update();
        }

        public static void Compile(this IEditable self)
        {
            string str = self.Codebox.GetString();
            // ここにstring型をProcess型に変換する処理を書く。
            // CodeParserで生成されたArrayListの中身は<move><X><Y><time>の形
            //<"if¥s*¥(¥s*touch¥s*¥)"><move><X><Y>

            CodeParser.yomitori(str);

            // self.SetProcesses(new Process[] {});


            //以下のリストの中身("move, x, y")を小集合とする
            var array = new List<string> { "size,1,1", "wait,1", "move,1,1,2" };


            //各小集合に対して、以下の分割処理を行う。
            foreach (var elements in array)
            {
                //小集合を要素に分割して、要素数1-4程度の配列を作成
                string[] tmp = elements.Split(',');

                //基本関数でなければ特殊処理
                if (tmp[0] != "size" && tmp[0] != "wait" && tmp[0] != "move")
                {
                    ConditionalAction(self,tmp);
                }

                BasicAction(self, tmp);

            }
        }

        //Processに関する基本関数
        public static void BasicAction(this IEditable self, string[] tmp)
        {
            switch (tmp[0])
            {
                //大きさ
                case "size":
                    self.AddProcess(new Process((obj, dt) => { obj.W = CellSize * float.Parse(tmp[1]); }));
                    self.AddProcess(new Process((obj, dt) => { obj.H = CellSize * float.Parse(tmp[2]); }));
                    break;

                //待機
                case "wait":
                    self.AddProcess(new Process((obj, dt) => { obj.VX = 0.0f; }));
                    self.AddProcess(new Process((obj, dt) => { obj.VY = 0.0f; }));
                    self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, float.Parse(tmp[1])));
                    break;

                //移動
                case "move":
                    self.AddProcess(new Process((obj, dt) => { obj.VX = CellSize * float.Parse(tmp[1]); }));
                    self.AddProcess(new Process((obj, dt) => { obj.VY = CellSize * float.Parse(tmp[2]); }));
                    self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, float.Parse(tmp[3])));
                    break;

                default:
                    break;
            }
        }

        //様々な条件があるときの動作
        public static void ConditionalAction(this IEditable self, string[] tmp)
        {
            //条件達成時に行われるべき配列を作る


            switch (tmp[0])
            {
                //オブジェクトに当たった時の判定
                case "touch":

                    self.AddProcess(new Process((obj, dt) =>
                    {

                    }));
                    break;
                //オブジェクトに乗った時の判定
                case "ontop":

                    //オブジェクト上部に判定エリアをつける
                    var judge_area = new GameObject();
                    judge_area.MidX = self.MidX;
                    judge_area.MidY = self.Y;
                    judge_area.W = self.W;
                    judge_area.H = CellSize / 8.0f;

                    //判定エリアにいるかどうかで処理するかを決める
                    if (self.CollidesWith(judge_area))
                    {

                    }
                    break;


                default:
                    break;
            }

        }


    }

}
