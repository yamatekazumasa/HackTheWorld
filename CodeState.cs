using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace HackTheWorld
{
    /// <summary>
    /// CodeBox に紐つけられたコードの状態を保存する。
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CodeState
    {
        [JsonProperty("cursor", Order = 0)]
        public int Cursor { get; set; }
        [JsonProperty("maxline", Order = 1)]
        public int MaxLine { get; set; }
        [JsonProperty("text", Order = 3)]
        public StringBuilder Text { get; set; }
        [JsonProperty("updatedAt", Order = 4)]
        public DateTime UpdatedAt { get; set; }

        public string[] Lines => Text.ToString().Split('\n');

        /// <summary>
        ///  カーソルが何行目の何文字目にあるかを取得する。
        /// </summary>
        public Tuple<int, int> CursorPosition
        {
            get
            {
                string[] lines = Text.ToString().Split('\n');
                int line, sum;
                for (sum = line = 0; sum + lines[line].Length < Cursor; sum += lines[line++].Length + 1) { }
                return Tuple.Create(line, Cursor - sum);
            }
        }

        /// <summary>
        /// 渡されたカーソルが何行目の何文字目にあるかを取得する。
        /// </summary>
        public Tuple<int, int> Position(int cursor)
        {
            string[] lines = Text.ToString().Split('\n');
            int line, sum;
            for (sum = line = 0; sum + lines[line].Length < cursor; sum += lines[line++].Length + 1) { }
            return Tuple.Create(line, cursor - sum);
        }

        /// <summary>
        /// カーソルの位置と最大の行数を受け取って、CodeState を生成する。
        /// </summary>
        public CodeState(int cursor, int maxLine)
        {
            Cursor = cursor;
            MaxLine = maxLine;
            Text = new StringBuilder();
            for (int i = 0; i < maxLine - 1; i++) Text.Append('\n');
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (!Directory.Exists(@".\code")) Directory.CreateDirectory(@".\code");
            StreamWriter sw = new StreamWriter(@".\code\" + DateTime.Now.ToString("MMddHHmmss") + ".json", false, Encoding.GetEncoding("utf-8"));
            sw.Write(json);
            sw.Close();
        }


        /// <summary>
        /// 文章を受け取って、自身に貼り付ける。
        /// </summary>
        public void ReadFrom(string text)
        {
            Cursor = 0;
            MaxLine = text.Split('\n').Length;
            Text = new StringBuilder(text);
        }

    }
}
