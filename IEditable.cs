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

            //ちょっと動いてくれるか試すよ
            var strlist = new List<string>();
            
            //strlist.Add("move");
            //strlist.Add("50");
            //strlist.Add("100");
            //self.AddProcess(new Process((obj, dt) => { obj.Size = new Vector(float.Parse(strlist[1]), float.Parse(strlist[2])); }, 2.0f));



            for (int i = 0; i < strlist.Count; i++)
            {

                if (strlist.Contains<string>("move"))
                {
                    self.AddProcess(new Process((obj, dt) => { obj.Position = new Vector(float.Parse(strlist[i+1]), float.Parse(strlist[i+2])); }, 2.0f));
                }

                
                 //switch文でindexが合ってるはずなのにArgumentOutOfRangeExceptionエラー
                switch (strlist[i])
                {
                    case "move":
                        self.AddProcess(new Process((obj, dt) => { obj.X += float.Parse(strlist[i + 1]) * dt; }, 2.0f));
                        self.AddProcess(new Process((obj, dt) => { obj.Y += float.Parse(strlist[i + 2]) * dt; }, 2.0f));
                        break;
            
                    case "size":
                        self.AddProcess(new Process((obj, dt) => { obj.Size = new Vector(float.Parse(strlist[i + 1]), float.Parse(strlist[i + 2])); }, 2.0f));
                        break;

                    case "velocity":
                        self.AddProcess(new Process((obj, dt) => { obj.Position += new Vector(double.Parse(strlist[i + 1]), double.Parse(strlist[i + 2])); }, 2.0f));
                        break;

                    case "stamina":

                        break;

                    default:
                        break;
                
            }
            
            }


        }



    }

}
