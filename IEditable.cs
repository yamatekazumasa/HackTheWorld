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

        public static void Compile(this IEditable self, Stage s)
        {
            string str = self.Codebox.GetString();
            // ここにstring型をProcess型に変換する処理を書く。
            // CodeParserで生成されたArrayListの中身は<size,1,1><wait,1><move,1,1,2>の形

            //CodeParser.yomitori(str);

            #region CodeParser.yomitori(str)をProcess型に変換する処理


            //以下のリストの中身("move, x, y")を小集合とする

            //動作テスト用配列
            //var midcode = new List<string> { "size,1,1", "wait,1", "move,1,1,2" };

            //本実行用配列
            var midcode = new List<string>();
            midcode = CodeParser.yomitori(str).Cast<string>().ToList();



            //各小集合に対して、以下の分割処理を行う。
            foreach (var elements in midcode)
            {
                //小集合を要素に分割して、要素数1-4程度の配列を作成
                string[] tmp = elements.Split(',');


                #region 特殊な関数の場合の処理
                //基本関数でなければ特殊処理
                if (tmp[0] != "size" && tmp[0] != "wait" && tmp[0] != "move") //基本動作を例外として弾く
                {
                    //条件達成時に行われるべき配列ctmpを作る
                    List<string> listtmp = new List<string>();
                    listtmp.AddRange(tmp);
                    listtmp.RemoveAt(0);
                    string[] ctmp = listtmp.ToArray();

                    //もともとの配列tmp[0]の最初の文字で分岐
                    switch (tmp[0])
                    {
                        #region オブジェクトに当たった時の判定
                        case "touch":

                            switch (ctmp[0])
                            {
                                //プレイヤーが触れたら大きさを変更
                                case "size":
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        if (obj.CollidesWith(s.Player))
                                        {
                                            obj.W = CellSize * float.Parse(ctmp[1]);
                                            obj.H = CellSize * float.Parse(ctmp[2]);
                                        }
                                    }));
                                    break;

                                //プレイヤーが触れたら待機
                                //ProcessのMoveの秒数指定の仕様上たぶん使えないです
                                case "wait":
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        obj.VX = 0.0f;
                                        obj.VY = 0.0f;
                                    }));
                                    self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, float.Parse(ctmp[1])));
                                    break;

                                //プレイヤーが触れたら移動
                                case "move":
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        obj.VX = CellSize * float.Parse(ctmp[1]);
                                        obj.VY = CellSize * float.Parse(ctmp[2]);
                                    }));
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        if (obj.CollidesWith(s.Player))
                                            obj.Move(dt);
                                    }, float.Parse(ctmp[3])));
                                    break;

                            }

                            break;
                        #endregion

                        #region オブジェクトに乗った時の判定
                        case "ontop":

                            //オブジェクト上部に判定エリアをつける
                            var judge_area = new GameObject();
                            judge_area.MidX = self.MidX;
                            judge_area.MidY = self.Y - CellSize / 8.0f;
                            judge_area.W = self.W;
                            judge_area.H = CellSize / 8.0f;

                            //判定エリアにいるかどうかで処理するかを決める
                            switch (ctmp[0])
                            {
                                //プレイヤーが触れたら大きさを変更
                                case "size":
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        if (obj.CollidesWith(s.Player))
                                        {
                                            obj.W = CellSize * float.Parse(ctmp[1]);
                                            obj.H = CellSize * float.Parse(ctmp[2]);
                                        }
                                    }));
                                    break;

                                //プレイヤーが触れたら待機
                                //ProcessのMoveの秒数指定の仕様上たぶん使えないです
                                case "wait":
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        obj.VX = 0.0f;
                                        obj.VY = 0.0f;
                                    }));
                                    self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, float.Parse(ctmp[1])));
                                    break;

                                //プレイヤーが触れたら移動
                                case "move":
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        obj.VX = CellSize * float.Parse(ctmp[1]);
                                        obj.VY = CellSize * float.Parse(ctmp[2]);
                                    }));
                                    self.AddProcess(new Process((obj, dt) =>
                                    {
                                        if (obj.CollidesWith(s.Player))
                                            obj.Move(dt);
                                    }, float.Parse(ctmp[3])));
                                    break;

                            }

                            break;
                        #endregion

                        default:
                            break;

                    }
                }
                #endregion


                #region 基本的な動作関数
                switch (tmp[0])
                {
                    //大きさ
                    case "size":
                        self.AddProcess(new Process((obj, dt) =>
                        {
                            obj.W = CellSize * float.Parse(tmp[1]);
                            obj.H = CellSize * float.Parse(tmp[2]);
                        }));
                        break;

                    //待機
                    case "wait":
                        self.AddProcess(new Process((obj, dt) =>
                        {
                            obj.VX = 0.0f;
                            obj.VY = 0.0f;
                        }));
                        self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, float.Parse(tmp[1])));
                        break;

                    //移動
                    case "move":
                        self.AddProcess(new Process((obj, dt) =>
                        {
                            obj.VX = CellSize * float.Parse(tmp[1]);
                            obj.VY = CellSize * float.Parse(tmp[2]);
                        }));
                        self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, float.Parse(tmp[3])));
                        break;

                    default:
                        break;
                }
                #endregion

            }
            #endregion
        }


    }

}
