using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Stage
    {

        [JsonProperty("rows")]
        public int Rows { get; set; }
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

        // 本来ならゲーム内にはない処理
        public static void Save(Stage stage)
        {
            string json = JsonConvert.SerializeObject(stage, Formatting.Indented);
            if (!Directory.Exists(@".\stage")) Directory.CreateDirectory(@".\stage");
            StreamWriter sw = new StreamWriter(@".\stage\test.json", false, Encoding.GetEncoding("utf-8"));
            sw.Write(json);
            sw.Close();
        }

        public static Stage Load()
        {
            if (!File.Exists(@".\stage\test.json")) return null;
            StreamReader sr = new StreamReader(@".\stage\test.json", Encoding.GetEncoding("utf-8"));
            Stage tmp = JsonConvert.DeserializeObject<Stage>(sr.ReadToEnd());
            Stage stage = new Stage(tmp.Rows, tmp.Cols);
            foreach (var obj in tmp.Objects)
            {
                switch (obj.ObjectType)
                {
                    case ObjectType.Block:
                        {
                            if (obj.IsEditable)
                            {
                                var b = new PBlock(obj.X, obj.Y);
                                stage.Blocks.Add(b);
                                stage.EditableObjects.Add(b);
                                stage.Objects.Add(b);
                            }
                            else
                            {
                                Block b = new Block(obj.X, obj.Y);
                                stage.Blocks.Add(b);
                                stage.Objects.Add(b);
                            }
                            break;
                        }
                    case ObjectType.Enemy:
                        {
                            Enemy e = new Enemy(obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H);
                            stage.Enemies.Add(e);
                            stage.Objects.Add(e);
                            break;
                        }
                    case ObjectType.Item:
                    {
                        Item i = new Item(obj.X, obj.Y, 0, 0, obj.W, obj.H);
                        stage.Items.Add(i);
                        stage.Objects.Add(i);
                        break;
                    }
                    case ObjectType.Player:
                    {
                        var p = new Player();
                        stage.Player = p;
                        stage.Objects.Add(p);
                        break;
                    }

                }
            }
            sr.Close();
            return stage;
        }

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
                            new Process((obj, dt) => { } , 0.0f),

                            new Process((obj, dt) => { obj.VY = -CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.VY = 0; } , 2.0f),

                            new Process((obj, dt) => { obj.VY = +CellSize; }, 1.0f),
                            new Process((obj, dt) => { } , 2.0f),
                            new Process((obj, dt) => { obj.VY = 0; } , 0.01f),

                            new Process((obj, dt) => { obj.VX = -CellSize; }, 1.0f),
                            new Process((obj, dt) => { obj.VX = 0; } , 2.0f),

                            new Process((obj, dt) => { } , 2.0f),
                            new Process((obj, dt) => { obj.Y -= dt*CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.Y += dt*CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.X += dt*CellSize; }, 3.0f),
                            new Process((obj, dt) => { obj.X -= dt*CellSize; }, 3.0f),
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
                        var item = new Item(CellSize * ix, CellSize * iy);
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
