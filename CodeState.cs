﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    class CodeState
    {
        [JsonProperty("cursor", Order = 0)]
        public int Cursor { get; set; }
        [JsonProperty("maxline", Order = 1)]
        public int MaxLine { get; set; }
        [JsonProperty("name", Order = 2)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Constants.ObjectType Name { get; set; }
        [JsonProperty("text", Order = 3)]
        public StringBuilder Text { get; set; }
        [JsonProperty("updatedAt", Order = 4)]
        public DateTime UpdatedAt { get; set; }

        public string[] Lines => Text.ToString().Split('\n');

        // カーソルが何行目の何文字目にあるかを取得する。
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

        // 渡されたカーソルが何行目の何文字目にあるかを取得する。
        public Tuple<int, int> Position(int cursor)
        {
            string[] lines = Text.ToString().Split('\n');
            int line, sum;
            for (sum = line = 0; sum + lines[line].Length < cursor; sum += lines[line++].Length + 1) { }
            return Tuple.Create(line, cursor - sum);
        }

        public CodeState(int cursor, int maxLine)
        {
            Cursor = cursor;
            MaxLine = maxLine;
            Name = Constants.ObjectType.Block;
            Text = new StringBuilder();
            for (int i = 0; i < maxLine - 1; i++) Text.Append('\n');
        }

        public void ReadFrom(string text)
        {
            Cursor = 0;
            MaxLine = text.Split('\n').Length;
            Text = new StringBuilder(text);
        }

    }
}
