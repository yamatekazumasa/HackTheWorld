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

        private Player _player;
        private List<Block> _blocks;
        private List<EditableObject> _pblocks;
        private List<Enemy> _enemies;
        private List<Item> _items;
        private Stage _stage;

        public Stage()
        {
            Rows = 9;
            Cols = 16;
            Objects = new List<GameObject>();
        }

        public Stage(int r, int c)
        {
            Rows = r;
            Cols = c;
            Objects = new List<GameObject>();
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
            stage._blocks = new List<Block>();
            stage._enemies = new List<Enemy>();
            stage._items = new List<Item>();
            stage.Objects = new List<GameObject>();
            foreach (var obj in tmp.Objects)
            {
                switch (obj.ObjectType)
                {
                    case ObjectType.Block:
                        {
                            Block b = new Block(obj.X, obj.Y);
                            stage._blocks.Add(b);
                            stage.Objects.Add(b);
                            break;
                        }
                    case ObjectType.Enemy:
                        {
                            Enemy e = new Enemy(obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H);
                            stage._enemies.Add(e);
                            stage.Objects.Add(e);
                            break;
                        }
                    case ObjectType.Item:
                    {
                        Item i = new Item(obj.X, obj.Y, 0, 0, obj.W, obj.H);
                        stage._items.Add(i);
                        stage.Objects.Add(i);
                        break;
                    }

                }
            }
            sr.Close();
            return stage;
        }
    }
}
