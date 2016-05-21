﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// ステージの状態を保存するクラス。
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Stage
    {
        /// <summary>
        /// 横のマス数
        /// </summary>
        [JsonProperty("rows")]
        public int Rows { get; set; }
        /// <summary>
        /// 縦のマス数
        /// </summary>
        [JsonProperty("cols")]
        public int Cols { get; set; }
        [JsonProperty("objects")]
        public List<GameObject> Objects { get; set; }
        public Player Player { get; set; }
        public List<Block> Blocks { get; set; }
        public List<IEditable> EditableObjects { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Item> Items { get; set; }

        public Stage()
        {
            Rows = 9;
            Cols = 16;
            Objects = new List<GameObject>();
            Player = new Player();
            Blocks = new List<Block>();
            EditableObjects = new List<IEditable>();
            Enemies = new List<Enemy>();
            Items = new List<Item>();
        }

        /// <summary>
        /// 縦と横のマス数を受け取ってステージを作成する。
        /// </summary>
        public Stage(int r, int c)
        {
            Rows = r;
            Cols = c;
            Objects = new List<GameObject>();
            Player = new Player();
            Blocks = new List<Block>();
            EditableObjects = new List<IEditable>();
            Enemies = new List<Enemy>();
            Items = new List<Item>();
        }

        /// <summary>
        /// 自身のディープコピーを作成する
        /// と見せかけて、一度 json 形式にしてもう一度 Stage に変換している。
        /// CodeParser が完成していないと、Editable なオブジェクトの動きがコピーされない。
        /// </summary>
        public Stage Replica => Parse(JsonConvert.SerializeObject(this));

        /// <summary>
        /// ステージを保存する。
        /// </summary>
        public static void Save(Stage stage)
        {
            string json = JsonConvert.SerializeObject(stage, Formatting.Indented);
            if (!Directory.Exists(@".\stage")) Directory.CreateDirectory(@".\stage");
            StreamWriter sw = new StreamWriter(@".\stage\test.json", false, Encoding.GetEncoding("utf-8"));
            sw.Write(json);
            sw.Close();
        }

        /// <summary>
        /// 保存されたステージを読み込む。
        /// </summary>
        public static Stage Load()
        {
            if (!File.Exists(@".\stage\test.json")) return null;
            StreamReader sr = new StreamReader(@".\stage\test.json", Encoding.GetEncoding("utf-8"));
            string json = sr.ReadToEnd();
            sr.Close();
            return Parse(json);
        }

        /// <summary>
        /// json をステージに変換する。
        /// </summary>
        public static Stage Parse(string json)
        {
            var tmp = JObject.Parse(json);
            Stage stage = new Stage((int)tmp["rows"], (int)tmp["cols"]);
            foreach (var obj in tmp["objects"])
            {
                switch ((string)obj["type"])
                {
                    case "Block":
                        {
                            if (obj["code"] != null)
                            {
                                var b = new PBlock((float)obj["x"], (float)obj["y"]);
                                b.Codebox.Current.Text = new StringBuilder((string)obj["code"]);
                                stage.Blocks.Add(b);
                                stage.EditableObjects.Add(b);
                                stage.Objects.Add(b);
                            }
                            else
                            {
                                Block b = new Block((float)obj["x"], (float)obj["y"]);
                                stage.Blocks.Add(b);
                                stage.Objects.Add(b);
                            }
                            break;
                        }
                    case "Enemy":
                        {
                            Enemy e = new Enemy((float)obj["x"], (float)obj["y"], (float)obj["vx"], (float)obj["vy"], (float)obj["width"], (float)obj["height"]);
                            stage.Enemies.Add(e);
                            stage.Objects.Add(e);
                            break;
                        }
                    case "Item":
                        {
                            Item i = new Item((float)obj["x"], (float)obj["y"], 0, 0, (float)obj["width"], (float)obj["height"], (ItemEffects)Enum.Parse(typeof(ItemEffects), (string)obj["effect"]));
                            stage.Items.Add(i);
                            stage.Objects.Add(i);
                            break;
                        }
                    case "Player":
                        {
                            var p = new Player();
                            stage.Player = p;
                            stage.Objects.Add(p);
                            break;
                        }

                }
            }
            return stage;
        }

        /// <summary>
        /// デモステージを作成する。
        /// </summary>
        public static Stage CreateDemoStage()
        {
            Stage stage = new Stage(CellNumX, CellNumY);
            // マップの生成
            for (int iy = 0; iy < CellNumY; iy++)
            {
                for (int ix = 0; ix < CellNumX; ix++)
                {
                    if (Map[iy, ix] == 1)
                    {
                        var block = new Block(CellSize * ix, CellSize * iy);
                        stage.Objects.Add(block);
                        stage.Blocks.Add(block);
                    }
                    if (Map[iy, ix] == 11)
                    {
                        var pblock = new PBlock(CellSize * ix, CellSize * iy);
                        pblock.SetProcesses(new[] {
                            new Process((obj, dt) => { } , 1.0f),

                            new Process((obj, dt) => { obj.VY = -CellSize; }),
                            new Process((obj, dt) => { obj.Move(dt); }, 3.0f),
                            new Process((obj, dt) => { obj.VY = 0; }),
                            new Process((obj, dt) => { } , 1.0f),

                            new Process((obj, dt) => { obj.VY = +CellSize; }),
                            new Process((obj, dt) => { obj.Move(dt); }, 3.0f),
                            new Process((obj, dt) => { obj.VY = 0; }),

                            new Process((obj, dt) => { obj.VX = +CellSize; }),
                            new Process((obj, dt) => { obj.Move(dt); }, 3.0f),
                            new Process((obj, dt) => { obj.VX = 0; }),

                            new Process((obj, dt) => { obj.VX = -CellSize; }),
                            new Process((obj, dt) => { obj.Move(dt); }, 3.0f),
                            new Process((obj, dt) => { obj.VX = 0; }),

//                            new Process((obj, dt) => { } , 2.0f),
//                            new Process((obj, dt) => { obj.Y -= dt*CellSize; }, 3.0f),
//                            new Process((obj, dt) => { obj.Y += dt*CellSize; }, 3.0f),
//                            new Process((obj, dt) => { obj.X += dt*CellSize; }, 3.0f),
//                            new Process((obj, dt) => { obj.X -= dt*CellSize; }, 3.0f),
                        });
                        stage.Objects.Add(pblock);
                        stage.Blocks.Add(pblock);
                        stage.EditableObjects.Add(pblock);
                    }
                    if (Map[iy, ix] == 2)
                    {
                        var enemy = new Enemy(CellSize * ix, CellSize * iy);
                        stage.Objects.Add(enemy);
                        stage.Enemies.Add(enemy);
                    }
                    if (Map[iy, ix] == 3)
                    {
                        var item = new Item(CellSize * ix + CellSize/4, CellSize * iy + CellSize/2, ItemEffects.Bigger);
                        stage.Objects.Add(item);
                        stage.Items.Add(item);
                    }
                }
            }
            var player = new Player();
            stage.Player = player;
            stage.Objects.Add(player);

            return stage;
        }

    }
}
