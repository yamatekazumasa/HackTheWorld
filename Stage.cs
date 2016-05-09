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
    public class Stage
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public List<GameObject> Objects { get; set; }

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
            if (!File.Exists(@".\stage")) return null;
            StreamReader sr = new StreamReader(@".\stage\test.json", Encoding.GetEncoding("utf-8"));
            Stage tmp = JsonConvert.DeserializeObject<Stage>(sr.ReadToEnd());
            Stage stage = new Stage(tmp.Rows, tmp.Cols);
            stage.Objects = new List<GameObject>();
            foreach (var obj in tmp.Objects)
            {

//                if (obj.ObjectType == ObjectType.Block)
//                {
//                    stage.Objects.Add(new ProcessfulObject(obj.X, obj.Y, obj.W, obj.H));
//                }
//                else if (obj is Block)
//                {
//                    stage.Objects.Add(new Block(obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H));
//                }
//                else if (obj is Enemy)
//                {
//                    stage.Objects.Add(new Enemy(obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H));
//                }
//                else
//                {
                    stage.Objects.Add(new GameObject(obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H));
//                }

//                stage.Objects.Add((GameObject)Activator.CreateInstance(obj.Type, new { obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H }));
            }
            sr.Close();
            return stage;
        }
    }
}
