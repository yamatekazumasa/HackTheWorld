using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace HackTheWorld
{
    public static class CodeParser
    {
        #region メイン
        public static ArrayList ConvertCodebox(string originStr)
        {
            Hashtable hash = new Hashtable();

            ICollection valuecall = hash.Values;
            //連続で入力してデバックしたい
            hash.Clear();

            //行で分割
            //char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };
            char[] delimiterChars = { '\n' };

            //分割した文を入れるリストと結果を入れるリスト
            ArrayList sArray = new ArrayList();
            ArrayList resultArray = new ArrayList();

            string[] tmp = originStr.Split(delimiterChars);
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }

            string strResult = "";
            if(!isFunction(sArray))
            {
                resultArray.Clear();
                resultArray.Add("へんな書きかた");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }

            if(!isValidScript(sArray))
            {
                resultArray.Clear();
                resultArray.Add("構文エラー");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }

            JumpToFunction(sArray,resultArray,hash);

            strResult = ConvertArrayToString(resultArray);
            Console.WriteLine(strResult);
            return resultArray;
        }

        public static void JumpToFunction(ArrayList sArray,ArrayList resultArray,Hashtable hash)
        {
            for(int i = 0;i < sArray.Count;i++)
            {
                UpdateHash(sArray,i,hash);
                //i行目が関数で始まってるかどうか
                switch(ReadSentenceHead(sArray,i))
                {
                    case 1:
                        For(sArray,resultArray,i,hash);
                        i = EndOfFunction(sArray,i);
                        break;
                    case 2:
                        If(sArray,resultArray,i,hash);
                        i = EndOfFunction(sArray,i);
                        break;
                    case 3:
                        While(sArray,resultArray,i,hash);
                        i = EndOfFunction(sArray,i);
                        break;
                    default:
                        AssignmentHashValue(sArray,i,hash);
                        resultArray.Add(sArray[i]);
                        break;
                }
            }
            //「3=3」や「4++」を消したい
            ArrayList resultArray2 = new ArrayList();
            for(int i = 0;i < resultArray.Count;i++)
            {
                string s = (string)resultArray[i];
                if(!s.Contains("=") && !s.Contains("+")) resultArray2.Add(resultArray[i]);
            }
            resultArray.Clear();
            for(int i = 0;i < resultArray2.Count;i++)
            {
                resultArray.Add(resultArray2[i]);
            }
        }
        #endregion

        #region はじめのチェック
        public static bool isValidScript(ArrayList sArray)
        {
            //全体の関数(for,if,while)の数
            int countFunction = CounterOfFunction(sArray);
            //閉じるためのendの数
            int countEnd = CounterOfEnd(sArray);
            //カウント(forの中にforがいたら次のendが終わりじゃないので数えたい)
            int count = 0;

            if(countFunction != countEnd)
            {
                Console.WriteLine("関数とendの数が違う");
            }
            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                int j = 0;
                //i行目がforで始まってるかどうか
                if(FirstFor(sArray,i))
                {
                    count++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            if(!isFor(sArray,i))
                            {
                                Console.WriteLine("forとendはいるみたいだけど\n文の中身が違う");
                                return false;
                            }
                            countFunction--;
                            break;
                        }
                    }

                }
                //似たようなもん
                if(FirstIf(sArray,i))
                {
                    count++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        if(FirstFunction(sArray,j)) count++;
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            if(!isIf(sArray,i))
                            {
                                //WindowContext.Invoke((Action)(() => {
                                //    Console.WriteLine("ifとendはいるみたいだけど\n文の中身が違う");
                                //}));
                                Console.WriteLine("ifとendはいるみたいだけど\n文の中身が違う");
                                return false;
                            }
                            countFunction--;
                            break;
                        }
                    }
                }
                if(FirstWhile(sArray,i))
                {
                    count++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        if(FirstFunction(sArray,j)) count++;
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            countFunction--;
                            break;
                        }
                    }
                }
                if(count != 0)
                {
                    Console.WriteLine("endが多い");
                    return false;
                }
                //関数が全部なくなったら
                if(countFunction == 0) return true;
            }
            //ループ回り切ったらだめ
            Console.WriteLine("関数とendの対応がだめ");
            return false;
        }

        public static bool isFunction(ArrayList sArray)
        {
            string msg = "";
            string s = "";
            int count = 0;
            int size = 14;
            //意味ない言葉が混ざっていないか見たい
            //"size,1,1", "wait,1", "move,1,1,2"
            Regex[] reg = new Regex[size];
            reg[0] = new Regex(@"\s*size\s*,\s*[\w+|\+|\-|\*|\/]+\s*,\s*[\w+|\+|\-|\*|\/]+");
            reg[1] = new Regex(@"\s*wait\s*,\s*\w+");
            reg[2] = new Regex(@"\s*move\s*,\s*[\w+|\+|\-|\*|\/]+\s*,\s*[\w+|\+|\-|\*|\/]+,\s*[\w+|\+|\-|\*|\/]+");
            reg[3] = new Regex(@"\s*\w+\s*=\s*[\w+|\+|\-|\*|\/]+\s*");
            reg[4] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*=");
            reg[5] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\+");
            reg[6] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\-");
            reg[7] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\=");
            reg[8] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\=");
            reg[9] = new Regex(@"for");
            reg[10] = new Regex(@"if");
            reg[11] = new Regex(@"while");
            reg[12] = new Regex(@"end");
            reg[13] = new Regex(@"else");

            Match[] mat = new Match[size];

            for(int i = 0;i < sArray.Count;i++)
            {
                count = 0;
                s = sArray[i].ToString();
                //matchの配列の初期化
                for(int j = 0;j < size;j++)
                {
                    mat[j] = reg[j].Match(s);
                }
                //エラーメッセージを吐いてほしい
                if(s.Contains("size") && mat[0].Length == 0) msg = "size";
                if(s.Contains("wait") && mat[1].Length == 0) msg = "wait";
                if(s.Contains("move") && mat[2].Length == 0) msg = "move";

                if(msg.Length != 0)
                {
                    Console.WriteLine(msg + "の書き方がまちがってます");
                    return false;
                }
                for(int j = 0;j < size;j++)
                {
                    if(mat[j].Length != 0) count++;
                }

                //すべてのmatchに当てはまらなかった
                if(count == 0)
                {
                    Console.WriteLine("知らない形の文が" + (i + 1) + "行目にあります");
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 繰り返し出てくる処理
        //sArray[home]がforなどである場合、そのforに対応したendの次の行が何行目なのか知りたい
        static int EndOfFunction(ArrayList sArray,int home)
        {
            int count = 1;
            int j = 0;
            for(j = home + 1;j < sArray.Count;j++)
            {
                if(FirstFunction(sArray,j)) count++;
                if(FirstEnd(sArray,j)) count--;
                if(count == 0)
                {
                    break;
                }
            }
            return j;
        }
        //最初が何で始まるかに応じて1か2か3で返す
        public static int ReadSentenceHead(ArrayList sArray,int i)
        {
            if(FirstFor(sArray,i)) return 1;
            if(FirstIf(sArray,i)) return 2;
            if(FirstWhile(sArray,i)) return 3;
            return 0;
        }
        //四則演算を行う
        public static string FourOperations(string s)
        {
            if(System.Text.RegularExpressions.Regex.IsMatch(s,@"\d+|[\+|\-|\*|\/]+") && !s.StartsWith(@"[\+|\-|\*|\/]") && !s.EndsWith(@"[\+|\-|\*|\/]") && !s.Contains(@"[\+\+|\-\-|\*\*|\/\/]"))
            {
                //ここで計算
                System.Data.DataTable dt = new System.Data.DataTable();

                //Type t = dt.Compute(s,"").GetType();

                return dt.Compute(s,"").ToString();
            }
            return "四則演算が変";
        }

        //ArrayListを\nで区切りながらstringに入れる
        static string ConvertArrayToString(ArrayList sArray)
        {
            string str = "";
            for(int i = 0;i < sArray.Count;i++)
            {
                str += (string)sArray[i] + "\n";
            }
            return str;
        }
        //ArrayListの中の関数の数を数える
        static int CounterOfFunction(ArrayList sArray)
        {
            int count = new int();
            for(int i = 0;i < sArray.Count;i++)
            {
                //関数の数を数える(今はforとif)
                if(FirstFunction(sArray,i)) count++;
            }
            return count;
        }
        //ArrayListの中のendの数を数える
        static int CounterOfEnd(ArrayList sArray)
        {
            int count = new int();
            for(int i = 0;i < sArray.Count;i++)
            {
                //関数の数を数える(今はforとif)
                if(FirstEnd(sArray,i)) count++;
            }
            return count;
        }
        //forなどにおいてhome行目の関数からendまでを抜き出して別のarraylistに入れたいときに使う
        static ArrayList CopyArrayList(ArrayList sArray,int home)
        {
            int i = 1;
            int count = 1;
            ArrayList tArray = new ArrayList();
            while(true)
            {
                if(FirstFunction(sArray,home + i)) count++;
                if(FirstEnd(sArray,home + i)) count--;

                tArray.Add(sArray[home + i]);

                if(count == 0) break;
                i++;
            }
            return tArray;
        }
        #endregion

        #region 関数の位置をタプルで返す
        public static Tuple<int,int>[] RowNumberOfFor(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] tmp = s.Split('\n');
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }

            ArrayList forArray = new ArrayList();

            int count = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(FirstFor(sArray,i))
                {
                    count++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            forArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] result = new Tuple<int,int>[forArray.Count];
            for(int i = 0;i < forArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)forArray[i];
            }
            return result;
        }
        public static Tuple<int,int>[] RowNumberOfIf(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] tmp = s.Split('\n');
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }
            ArrayList ifArray = new ArrayList();

            int count = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(FirstIf(sArray,i))
                {
                    count++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            //tupleの中に行番号を入れる
                            ifArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] result = new Tuple<int,int>[ifArray.Count];
            for(int i = 0;i < ifArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)ifArray[i];
            }
            return result;
        }
        public static Tuple<int,int>[] RowNumberOfWhile(string s)
        {
            ArrayList sArray = new ArrayList();

            string[] tmp = s.Split('\n');
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }
            ArrayList whileArray = new ArrayList();

            int count = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(FirstWhile(sArray,i))
                {
                    count++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            //tupleの中に行番号を入れる
                            whileArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] result = new Tuple<int,int>[whileArray.Count];
            for(int i = 0;i < whileArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)whileArray[i];
            }
            return result;
        }
        public static Tuple<int,int>[] RowNumberOfFunction(string s)
        {
            ArrayList allArray = new ArrayList();

            Tuple<int,int>[] t1 = RowNumberOfFor(s);
            Tuple<int,int>[] t2 = RowNumberOfIf(s);
            Tuple<int,int>[] t3 = RowNumberOfWhile(s);

            for(int i = 0;i < t1.Length;i++)
            {
                allArray.Add(t1[i]);
            }
            for(int i = 0;i < t2.Length;i++)
            {
                allArray.Add(t2[i]);
            }
            for(int i = 0;i < t3.Length;i++)
            {
                allArray.Add(t3[i]);
            }

            Tuple<int,int>[] result = new Tuple<int,int>[allArray.Count];
            for(int i = 0;i < allArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)allArray[i];
            }
            return result;
        }
        #endregion

        #region hash関係
        public static void UpdateHash(ArrayList sArray,int i,Hashtable hash)
        {
            string str1, str2, str3;
            ICollection keycall = hash.Keys;

            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>\w+)\s*=\s*(?<right_hand>[(?<value>\w+)|\+|\-|\*|\/|\.]+)\s*"))
            {
                string s = (string)sArray[i];
                Regex reg = new Regex(@"\s*(?<name>\w+)\s*=\s*(?<right_hand>.*)");
                Match mat = reg.Match(s);
                str1 = mat.Groups["right_hand"].Value;

                foreach(string k in keycall)
                {
                    //右辺にすでにハッシュに入れたものがいる
                    if(str1.Contains(k))
                    {
                        string pattern = k;
                        string replacement = hash[k].ToString();
                        Regex r = new Regex(pattern);
                        s = r.Replace(s,replacement);
                    }
                }
                //右辺に今までハッシュに登録されていない文字がいる
                if(str1.Contains(@"\w+")) return;

                reg = new Regex(@"\s*(?<name>\w+)\s*=\s*(?<right_hand>[\d+|\+|\-|\*|\/|\.]+)\s*");
                mat = reg.Match(s);
                str2 = mat.Groups["right_hand"].Value;
                //右辺の文字は数字に置換されたはずなので四則演算の関数に入れてよい
                str2 = FourOperations(str2);

                //hashへの登録
                str3 = mat.Groups["name"].Value;
                hash[str3] = str2;
                return;

            }
            //ここから＋＋とか＋＝とかの部分
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\+\+"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\+");
                Match m = r.Match((string)sArray[i]);
                str1 = m.Groups["name"].Value;
                hash[str1] = Convert.ToInt32(hash[str1]) + 1;
                return;
            }
            if(Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\-\-"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\-\-");
                Match m = r.Match((string)sArray[i]);
                str1 = m.Groups["name"].Value;
                hash[str1] = Convert.ToInt32(hash[str1]) - 1;
                return;
            }
            if(Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>\d+)"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>\d+)");
                Match m = r.Match((string)sArray[i]);
                str1 = m.Groups["name"].Value;
                str2 = m.Groups["value"].Value;
                hash[str1] = Convert.ToInt32(hash[str1]) + int.Parse(str2);
                return;
            }
            if(Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\-\=\s*(?<value>\d+)"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>\d+)");
                Match m = r.Match((string)sArray[i]);
                str1 = m.Groups["name"].Value;
                str2 = m.Groups["value"].Value;
                hash[str1] = Convert.ToInt32(hash[str1]) - int.Parse(str2);
                return;
            }

        }

        public static void AssignmentHashValue(ArrayList sArray,int x,Hashtable hash)
        {
            ICollection keycall = hash.Keys;
            //hashになにか入ってたら
            if(keycall.Count > 0)
            {
                //hashのkeyごとに
                foreach(string key in keycall)
                {
                    string input = (string)sArray[x];
                    int foundIndex = input.IndexOf(key);
                    //inputのどこかにハッシュのキーがいる限り回る
                    while(0 <= foundIndex)
                    {
                        bool canAssignment = true;
                        string s;

                        //左右に余計な文字がついている:一番はじめや一番後ろでマッチしたときは見る場所がずれる
                        if(foundIndex == 0)
                        {
                            s = input.Substring(foundIndex,key.Length + 1);
                            if(Regex.IsMatch(s,@"[a-zA-Z]$")) canAssignment = false;
                        }
                        else if(foundIndex != input.Length - 1)
                        {
                            s = input.Substring(foundIndex - 1,key.Length + 2);
                            if(Regex.IsMatch(s,@"^[a-zA-Z]") || Regex.IsMatch(s,@"[a-zA-Z]$")) canAssignment = false;
                        }
                        else
                        {
                            s = input.Substring(foundIndex - 1,key.Length + 1);
                            if(Regex.IsMatch(s,@"^[a-zA-Z]")) canAssignment = false;
                        }

                        if(canAssignment)
                        {
                            //charの配列をlistに追加していき、最後にstringにしたい
                            char[] c = input.ToCharArray();
                            char[] cHashValue = hash[key].ToString().ToCharArray();
                            List<char> cList = new List<char>();

                            //はじめにkeyがいないときはもともとの文が最初に入る
                            if(foundIndex != 0)
                            {
                                for(int i = 0;i < foundIndex;i++)
                                {
                                    cList.Add(c[i]);
                                }
                            }
                            //hashの中身の値をリストに加える
                            for(int i = 0;i < cHashValue.Length;i++)
                            {
                                cList.Add(cHashValue[i]);
                            }
                            //keyの後ろの残りの文を入れる
                            if(foundIndex + key.Length < c.Length)
                            {
                                for(int i = foundIndex + key.Length;i < c.Length;i++)
                                {
                                    cList.Add(c[i]);
                                }
                            }
                            string result = "";
                            for(int i = 0;i < cList.Count;i++)
                            {
                                result += cList[i];
                            }
                            //hashの値を代入した後の文に差し替える
                            sArray[x] = result;
                        }

                        //次の検索開始位置を決める
                        input = (string)sArray[x];
                        int nextIndex = foundIndex + key.Length;
                        if(nextIndex < input.Length)
                        {
                            //次の位置を探す
                            foundIndex = input.IndexOf(key,nextIndex);
                        }
                        else
                        {
                            //最後まで検索したので終わる
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region for if while
        public static void For(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            int typeOfFor = 0;
            //sArray[home]はforから始まっていて繰り返し回数を指定している行
            //どんな書き方をしているかの正規表現を用いた場合分けをしたい
            //いちいち間に\s*(0個以上の空白文字を示す)を入れて間に空白が入っても読めるようにする
            //typeを3つ作ることにする

            //for(i=0;i<5;i++)がtype1
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\(\s*\w+\s*\=\s*\w+\s*;\s*\w+\s*" + @"<|>|(<=)|(>=)|(==)" + @"\s*\w+\s*;\s*\w+[\+\+|\-\-|\+=\w+|\-=\w+]\)")) typeOfFor = 1;
            //for i=0 to 3がtype2
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+\s*=\s*\w+\s*to\s*\w+")) typeOfFor = 2;
            //for 2とかをtype3とする
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+")) typeOfFor = 3;

            switch(typeOfFor)
            {
                case 1:
                    Regex reg1_1 = new Regex(@"(?<start>\w+\s*\=\s*\w+)");
                    Regex reg1_2 = new Regex(@"(?<condition>\w+\s*(<|>|(<=)|(>=)|(==))\s*\w+)");
                    Regex reg1_3 = new Regex(@"(?<update>\w+(\+\+)|(\-\-)|(\+=\w+)|(\-=\w+))");
                    Match m1_1 = reg1_1.Match((string)sArray[home]);
                    Match m1_2 = reg1_2.Match((string)sArray[home]);
                    Match m1_3 = reg1_3.Match((string)sArray[home]);
                    //代入して条件式などが失われると困るので別のArrayListに入れておきたい
                    ArrayList tArray = new ArrayList();
                    ArrayList uArray = new ArrayList();
                    bool breakIsExists = false;

                    //homeまで読む(条件取り終わったから)
                    UpdateHash(sArray,home,hash);

                    //初期条件を別のArrayListに入れ、hashを更新する
                    tArray.Add(m1_1.Groups["start"].Value);
                    UpdateHash(tArray,0,hash);

                    //大小判定をしたいので、条件式に代入する
                    uArray.Add(m1_2.Groups["condition"].Value);
                    AssignmentHashValue(uArray,0,hash);
                    //ここからループ、条件式の大小判定があっているか確かめる
                    while(SizeComparing((string)uArray[0]))
                    {
                        tArray = CopyArrayList(sArray,home);
                        //最後に更新式をつけておく
                        tArray.Insert(tArray.Count - 1,m1_3.Groups["update"].Value);

                        //1行ずつ読む
                        for(int i = 0;i < tArray.Count;i++)
                        {
                            while(!FirstEnd(tArray,i))
                            {
                                UpdateHash(tArray,i,hash);

                                switch(ReadSentenceHead(tArray,i))
                                {
                                    case 1:
                                        For(tArray,result,i,hash);
                                        i = EndOfFunction(tArray,i) + 1;
                                        break;
                                    case 2:
                                        If(tArray,result,i,hash);
                                        i = EndOfFunction(tArray,i) + 1;
                                        break;
                                    case 3:
                                        While(tArray,result,i,hash);
                                        i = EndOfFunction(tArray,i) + 1;
                                        break;
                                    default:
                                        AssignmentHashValue(tArray,i,hash);
                                        result.Add(tArray[i]);
                                        i++;
                                        break;
                                }
                                //breakが結果に入っていたらbreakしたい
                                if(result.Count != 0)
                                {
                                    if(result[result.Count - 1].ToString() == "break")
                                    {
                                        breakIsExists = true;
                                        result.RemoveAt(result.Count - 1);
                                        break;
                                    }
                                }
                            }
                            if(breakIsExists) break;
                        }
                        if(breakIsExists) break;
                        //条件式をまた入れ、最新の値で代入しておく
                        uArray.Clear();
                        uArray.Add(m1_2.Groups["condition"].Value);
                        AssignmentHashValue(uArray,0,hash);
                    }
                    return;

                case 3:
                    Regex reg3 = new Regex(@"for\s*(?<repeat>\w+)");
                    Match m3 = reg3.Match((string)sArray[home]);

                    int repeatCount = 0;
                    //hashの値を用いるか、数字として読むかして繰り返し回数を決める
                    if(hash.ContainsKey(m3.Groups["repeat"].Value)) repeatCount = Convert.ToInt32(hash[m3.Groups["repeat"].Value]);
                    else if(!int.TryParse((string)m3.Groups["repeat"].Value,out repeatCount))
                    {
                        Console.WriteLine("(For type3)数字を代入していますか？");
                        return;
                    }
                    breakIsExists = false;

                    //これも代入によって原文が書き換わらないようにするためのarraylist
                    tArray = new ArrayList();

                    UpdateHash(sArray,home,hash);
                    for(int i = 0;i < repeatCount;i++)
                    {
                        tArray = CopyArrayList(sArray,home);
                        int j = 0;
                        while(!FirstEnd(tArray,j))
                        {
                            UpdateHash(tArray,j,hash);

                            switch(ReadSentenceHead(tArray,j))
                            {
                                case 1:
                                    For(tArray,result,j,hash);
                                    j = EndOfFunction(tArray,j) + 1;
                                    break;
                                case 2:
                                    If(tArray,result,j,hash);
                                    j = EndOfFunction(tArray,j) + 1;
                                    break;
                                case 3:
                                    While(tArray,result,j,hash);
                                    j = EndOfFunction(tArray,j) + 1;
                                    break;
                                default:
                                    AssignmentHashValue(tArray,j,hash);
                                    result.Add(tArray[j]);
                                    j++;
                                    break;
                            }
                            if(result.Count != 0)
                            {
                                if(result[result.Count - 1].ToString() == "break")
                                {
                                    breakIsExists = true;
                                    result.RemoveAt(result.Count - 1);
                                    break;
                                }
                            }
                        }
                        if(breakIsExists) break;
                    }

                    return;
                default:
                    result.Add("whileがうまくいっていない");
                    return;
            }
        }


        public static void If(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            UpdateHash(sArray,home,hash);

            ArrayList tArray = new ArrayList();

            //条件を抜き出してarraylistへ
            string s = (string)sArray[home];
            Regex reg = new Regex(@"(?<condition>\w+\s*(<|>|(<=)|(>=)|(==))\s*\d+)");
            Match m = reg.Match(s);
            tArray.Add(m.Value);
            AssignmentHashValue(tArray,0,hash);


            if(SizeComparing((string)tArray[0]))
            {
                int i = 1;
                //行のはじめにendがくるかbreakがくるまで読む
                while(!FirstEnd(sArray,home + i) && !FirstBreak(sArray,home + i - 1))
                {
                    if(FirstElse(sArray,home + i)) break;
                    UpdateHash(sArray,home + i,hash);

                    switch(ReadSentenceHead(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home+i);
                            break;
                        case 2:
                            If(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        case 3:
                            While(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        default:
                            AssignmentHashValue(sArray,home + i,hash);
                            result.Add(sArray[home + i]);
                            i++;
                            break;
                    }
                }
            }
            else
            {
                //if文がfalseになったら
                int tmp = 0;
                int i = 0;
                //elseがいるか探す、いなかったらこのif文はやることがないのでreturnする
                while(true)
                {
                    if(FirstElse(sArray,home + tmp))
                    {
                        i = tmp + 1;
                        break;
                    }
                    tmp++;
                    if(home + tmp >= sArray.Count) return;
                }
                //elseの行からまた読み直す
                while(!FirstEnd(sArray,home + i) && !FirstBreak(sArray,home + i - 1))
                {
                    //home+iまで同じことをする
                    UpdateHash(sArray,home + i,hash);

                    switch(ReadSentenceHead(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home+i);
                            break;
                        case 2:
                            If(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        case 3:
                            While(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        default:
                            AssignmentHashValue(sArray,home + i,hash);
                            result.Add(sArray[home + i]);
                            i++;
                            break;
                    }
                }
            }
            return;
        }

        public static void While(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            ArrayList tArray = new ArrayList();
            ArrayList uArray = new ArrayList();

            //条件を抜き出す
            string s = (string)sArray[home];
            Regex r = new Regex(@"(?<condition>\w+\s*(<|>|(<=)|(>=)|(==))\s*\d+)");
            Match m = r.Match(s);
            bool breakIsExists = false;

            //homeまで読んでhash登録、代入、forとendの対応の取り直し
            UpdateHash(sArray,home,hash);

            while(!breakIsExists)
            {
                //uArrayは条件式だけをいれるためにいる
                //ループが回るたびに条件式を入れなおし、代入しなおす
                uArray.Clear();
                uArray.Add(m.Value);
                AssignmentHashValue(uArray,0,hash);

                if(SizeComparing((string)uArray[0]))
                {
                    //tArrayにsArrayのすべてをコピー
                    tArray.Clear();
                    for(int j = 0;j < sArray.Count;j++)
                    {
                        tArray.Add(sArray[j]);
                    }


                    int i = 1;
                    while(!FirstEnd(sArray,home + i) && !FirstBreak(sArray,home + i - 1))
                    {
                        //home+iまで同じことをする
                        UpdateHash(sArray,home + i,hash);

                        switch(ReadSentenceHead(sArray,home + i))
                        {
                            case 1:
                                For(sArray,result,home + i,hash);
                                i = EndOfFunction(sArray,home+i);
                                break;
                            case 2:
                                If(sArray,result,home + i,hash);
                                i = EndOfFunction(sArray,home + i);
                                break;
                            case 3:
                                While(sArray,result,home + i,hash);
                                i = EndOfFunction(sArray,home + i);
                                break;
                            default:
                                AssignmentHashValue(tArray,home + i,hash);
                                result.Add(tArray[home + i]);
                                i++;
                                break;
                        }
                        if(result.Count != 0)
                        {
                            if(result[result.Count - 1].ToString() == "break")
                            {
                                breakIsExists = true;
                                result.RemoveAt(result.Count - 1);
                                break;
                            }
                        }
                    }

                }
                else break;
            }
        }

        #endregion

        #region bool群
        public static bool isFor(ArrayList sArray,int home)
        {
            //一致してるかは知りたいけどうしろに余計なのがついてたらはじきたい
            if(Regex.IsMatch((string)sArray[home],@"^for\s*\(\s*\w+\s*\=\s*\w+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\w+\s*;\s*\w+[\+\+|\-\-|\+=\w+|\-=\w+]\)\s*$")) return true;
            if(Regex.IsMatch((string)sArray[home],@"^for\s*\w+\s*=\s*\w+\s*to\s*\w+\s*$")) return true;
            if(Regex.IsMatch((string)sArray[home],@"^for\s*\w+\s*$")) return true;
            return false;
        }

        public static bool isIf(ArrayList sArray,int home)
        {
            string[] reg = new string[5];
            reg[0] = @"[0-9a-zA-Z]+\s*\<\s*[0-9a-zA-Z]+";
            reg[1] = @"[0-9a-zA-Z]+\s*\>\s*[0-9a-zA-Z]+";
            reg[2] = @"[0-9a-zA-Z]+\s*\<\s*\=\s*[0-9a-zA-Z]+";
            reg[3] = @"[0-9a-zA-Z]+\s*\>\s*\=\s*[0-9a-zA-Z]+";
            reg[4] = @"[0-9a-zA-Z]+\s*\=\s*\=\s*[0-9a-zA-Z]+";

            for(int i = 0;i < reg.Length;i++)
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"if\s*\(" + reg[i] + @"\)\s*"))
                {
                    if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"if\s*\(" + reg[i] + @"\)\s*."))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        static bool FirstFunction(ArrayList sArray,int i)
        {
            if(FirstFor(sArray,i) || FirstIf(sArray,i) || FirstWhile(sArray,i)) return true;
            return false;
        }
        //この辺はifの中に書くとき短くしたいからいる
        public static bool FirstFor(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("for")) return true;
            return false;
        }
        public static bool FirstIf(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("if")) return true;
            return false;
        }
        public static bool FirstWhile(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("while")) return true;
            return false;
        }
        public static bool FirstEnd(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("end")) return true;
            return false;
        }
        public static bool FirstElse(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("else")) return true;
            return false;
        }
        public static bool FirstBreak(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("break")) return true;
            return false;
        }

        //ifのための判定群
        public static bool SizeComparing(string s)
        {
            //大小判定したい
            Regex reg1 = new Regex(@"(?<left_hand>\d+)\s*<\s*(?<right_hand>\d+)");
            Match m1 = reg1.Match(s);

            Regex reg2 = new Regex(@"(?<left_hand>\d+)\s*>\s*(?<right_hand>\d+)");
            Match m2 = reg2.Match(s);

            Regex reg3 = new Regex(@"(?<left_hand>\d+)\s*<\s*=\s*(?<right_hand>\d+)");
            Match m3 = reg3.Match(s);

            Regex reg4 = new Regex(@"(?<left_hand>\d+)\s*>\s*=\s*(?<right_hand>\d+)");
            Match m4 = reg4.Match(s);

            Regex reg5 = new Regex(@"(?<left_hand>\d+)\s*=\s*=\s*(?<right_hand>\d+)");
            Match m5 = reg5.Match(s);

            if(m1.Length > 0)
            {
                int left_hand = int.Parse(m1.Groups["left_hand"].Value);
                int right_hand = int.Parse(m1.Groups["right_hand"].Value);
                if(left_hand < right_hand) return true;
                else return false;
            }
            if(m2.Length > 0)
            {
                int left_hand = int.Parse(m2.Groups["left_hand"].Value);
                int right_hand = int.Parse(m2.Groups["right_hand"].Value);
                if(left_hand > right_hand) return true;
                else return false;
            }
            if(m3.Length > 0)
            {
                int left_hand = int.Parse(m3.Groups["left_hand"].Value);
                int right_hand = int.Parse(m3.Groups["right_hand"].Value);
                if(left_hand <= right_hand) return true;
                else return false;
            }
            if(m4.Length > 0)
            {
                int left_hand = int.Parse(m4.Groups["left_hand"].Value);
                int right_hand = int.Parse(m4.Groups["right_hand"].Value);
                if(left_hand >= right_hand) return true;
                else return false;
            }
            if(m5.Length > 0)
            {
                int left_hand = int.Parse(m5.Groups["left_hand"].Value);
                int right_hand = int.Parse(m5.Groups["right_hand"].Value);
                if(left_hand == right_hand) return true;
                else return false;
            }
            return false;
        }
        #endregion

        #region 使ってない
        //消すのが惜しかった
        public static int CountChar(string s,char c)
        {
            return s.Length - s.Replace(c.ToString(),"").Length;
        }
        //ノーマルかっこと閉じかっこ、中かっこと中閉じかっこの数が同じかどうか(いらない気がしてきた)
        public static bool kakkocounter(ArrayList sArray)
        {
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0;
            for(int i = 0;i < sArray.Count;i++)
            {
                count1 += CountChar(sArray[i].ToString(),'(');
                count2 += CountChar(sArray[i].ToString(),')');
                count3 += CountChar(sArray[i].ToString(),'{');
                count4 += CountChar(sArray[i].ToString(),'}');
            }
            if(count1 == count2 && count3 == count4) return true;
            return false;
        }
        #endregion

    }
}
