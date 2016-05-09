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
        private List<GameObject> objects;
        private int rows;
        private int cols;

        public int Rows => rows;
        public int Cols => cols;
        public List<GameObject> Objects { get { return objects; } set { objects = value; } }

        public Stage()
        {
            rows = 9;
            cols = 16;
            objects = new List<GameObject>();
        }

        public Stage(Stage stage)
        {
            rows = stage.Rows;
            cols = stage.Cols;
            objects = new List<GameObject>();
            foreach (var obj in stage.objects)
            {
                objects.Add(obj);
            }
        }

        public Stage(int r, int c)
        {
            rows = r;
            cols = c;
            objects = new List<GameObject>();
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
            Stage stage = new Stage(tmp.rows, tmp.cols);
            stage.objects = new List<GameObject>();
            foreach (var obj in tmp.objects)
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
//                    stage.Objects.Add(new GameObject(obj.X, obj.Y, obj.VX, obj.VY, obj.W, obj.H));
//                }

//                stage.Objects.Add((GameObject)Activator.CreateInstance(obj.Type, new { obj.X, obj.Y }));
            }
            sr.Close();
            return stage;
        }
    }
}
