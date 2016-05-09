using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public Stage(int r, int c)
        {
            rows = r;
            cols = c;
            objects = new List<GameObject>();
        }

        public static void Save(Stage stage)
        {
            string json = JsonConvert.SerializeObject(stage, Formatting.Indented);
            StreamWriter sw = new StreamWriter(@".\stage\test.json", false, Encoding.GetEncoding("utf-8"));
            sw.Write(json);
            sw.Close();
        }

        public static Stage Load()
        {
            StreamReader sr = new StreamReader(@".\stage\test.json", Encoding.GetEncoding("utf-8"));
            Stage stage = JsonConvert.DeserializeObject<Stage>(sr.ReadToEnd());
            sr.Close();
            return stage;
        }
    }
}
