using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;
using System.Text.RegularExpressions;


namespace HackTheWorld
{
    public static class CodeParser
    {
        //forとかifとかのかっこの組が配列の何行目か
        //arraylistのなかにTuple<int,int>をもたせる
        public static ArrayList forArray = new ArrayList();
        public static ArrayList ifArray = new ArrayList();
        //まとめた
        public static ArrayList funcArray = new ArrayList();
        static bool mismatch = false;
        static bool misfor = false;
        static bool misif = false;
        public static ArrayList yomitori(string s1)
        {
            //連続で入力してデバックしたいからいる奴ら
            forArray.Clear();
            ifArray.Clear();
            funcArray.Clear();
            mismatch = false;
            misfor = false;
            misif = false;

            //行で分割
            //char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };
            char[] delimiterChars = { '\n' };

            //分割した文を入れるリストと結果を入れるリスト
            ArrayList sArray = new ArrayList();
            ArrayList result = new ArrayList();


            string[] s2 = s1.Split(delimiterChars);
            for(int i = 0;i < s2.Length;i++)
            {
                sArray.Add(s2[i]);
            }
            //for、ifとendが組になってるかと組がどこの行か
            kakkoread(sArray);
            if(mismatch)
            {
                MessageBox.Show("関数の始まりと終わりの対応が乱れてる");
                result.Clear();
                result.Add("関数乱れマン");
                return result;
            }
            //割り振る
            warifuri(sArray,result);
            if(misfor)
            {
                MessageBox.Show("forとendはいるみたいだけど\n文の中身が違う");
                result.Clear();
                result.Add("for間違えマン");
                return result;
            }
            if(misif)
            {
                MessageBox.Show("ifとendはいるみたいだけど\n文の中身が違う");
                result.Clear();
                result.Add("if間違えマン");
                return result;
            }
            string str = "";
            for(int i = 0;i < result.Count;i++)
            {
                str += (string)result[i] + "\n";
            }
            MessageBox.Show(str);
            return result;
        }


        public static void kakkoread(ArrayList sArray)
        {
            int countfunction = 0;
            int kakko = 0;
            for(int i = 0;i < sArray.Count;i++)
            {
                //関数の数を数える(今はforとif)
                if(firstfor(sArray,i) || firstif(sArray,i)) countfunction++;
            }
            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(firstfor(sArray,i))
                {
                    kakko++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(firstfor(sArray,j) || firstif(sArray,j)) kakko++;
                        //endがいたらへらす
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            //tupleの中に行番号を入れる
                            forArray.Add(new Tuple<int,int>(i,j));
                            funcArray.Add(new Tuple<int,int>(i,j));
                            countfunction--;
                            break;
                        }
                    }
                }
                //似たようなもん
                if(firstif(sArray,i))
                {
                    kakko++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        if(firstfor(sArray,j) || firstif(sArray,j)) kakko++;
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            ifArray.Add(new Tuple<int,int>(i,j));
                            funcArray.Add(new Tuple<int,int>(i,j));
                            countfunction--;
                            break;
                        }
                    }
                }
                //関数っぽいのが全部なくなったら早くぬける
                if(countfunction == 0) break;
            }
            //ループ終わったのにcountfunctionが残ってたらmismatch
            if(countfunction != 0) mismatch = true;
        }


        //最初が何で始まるかわかるといちいち書かなくていいからべんり
        public static int bunki(ArrayList sArray,int i)
        {
            if(firstfor(sArray,i)) return 1;
            if(firstif(sArray,i)) return 2;
            if(firstwhile(sArray,i)) return 3;
            return 0;
        }


        public static void warifuri(ArrayList sArray,ArrayList result)
        {
            //基本1行ずつ読む
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目が関数で始まってるかどうか
                switch(bunki(sArray,i))
                {
                    case 1:
                        For(sArray,result,i);
                        i = nextend(i);
                        break;
                    case 2:
                        If(sArray,result,i);
                        i = nextend(i);
                        break;
                    default:
                        result.Add(sArray[i]);
                        break;
                }
            }
        }


        public static void For(ArrayList sArray,ArrayList result,int home)
        {
            if(!boolfor(sArray,home))
            {
                result.Add("boolforひっかかった");
                return;
            }
            int type = 0;
            //sArray[home]はforから始まっていて繰り返し回数を指定している行
            //どんな書き方をしているかの正規表現を用いた場合分けをしたい
            //いちいち間に\s*(0個以上の空白文字を示す)を入れて間に空白が入っても読めるようにする
            //typeを3つ作ることにする

            //for(i=0;i<5;i++)がtype1
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\(\s*\w+\s*\=\s*\d+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)")) type = 1;
            //for i=0 to 3がtype2
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+\s*=\s*\d+\s*to\s*\d+")) type = 2;
            //for 2とかをtype3とする
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\d+")) type = 3;

            switch(type)
            {
                //case 1:
                //    Regex re1_1 = new Regex(@"for\s*\(\s*\w+\s*\=\s*(?<start>\d+)\s*;\s*");
                //    Regex re1_2 = new Regex(@"(?<jouken>\w+\s*[<|>|<=|>=]\s*\d+)");
                //    Regex re1_3 = new Regex(@"(?<update>\w+[\+\+|\-\-|\+=\d+|\-=\d+])");


                case 3:
                    Regex re3 = new Regex(@"for\s*(?<repeat>\d+)");
                    Match m3 = re3.Match((string)sArray[home]);
                    int n = int.Parse(m3.Groups["repeat"].Value);
                    for(int i = 0;i < n;i++)
                    {
                        int j = 1;
                        while(!firstend(sArray,home + j))
                        {
                            switch(bunki(sArray,home + j))
                            {
                                case 1:
                                    For(sArray,result,home + j);
                                    j += nextend(home + j)-(home+j)+1;
                                    break;
                                case 2:
                                    If(sArray,result,home + j);
                                    j += nextend(home + j) - (home + j) + 1;
                                    break;
                                default:
                                    result.Add(sArray[home + j]);
                                    j++;
                                    break;
                            }
                        }
                    }

                    return;
                default:
                    result.Add("ここのfor boolfor通過しといてうまくいってない");
                    return;
            }
        }


        public static void If(ArrayList sArray,ArrayList result,int home)
        {
            if(!boolif(sArray,home))
            {
                result.Add("boolifひっかかった");
                return;
            }
            if(hantei((string)sArray[home]))
            {
                int i = 1;
                while(!firstend(sArray,home + i))
                {
                    if(firstelse(sArray,home + i)) break;
                    switch(bunki(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i);
                            i += nextend(home + i) - (home + i) + 1;
                            break;
                        case 2:
                            If(sArray,result,home + i);
                            i += nextend(home + i) - (home + i) + 1;
                            break;
                        default:
                            result.Add(sArray[home + i]);
                            i++;
                            break;
                    }
                }
            }
            else
            {
                int tmp = 0;
                int i = 0;
                while(true)
                {
                    if(firstelse(sArray,home+tmp))
                    {
                        i = tmp + 1;
                        break;
                    }
                    else tmp++;
                    if(home + tmp >= sArray.Count) return;
                }
                while(!firstend(sArray,home + i))
                {
                    switch(bunki(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i);
                            i += nextend(home + i) - (home + i) + 1;
                            break;
                        case 2:
                            If(sArray,result,home + i);
                            i += nextend(home + i) - (home + i) + 1;
                            break;
                        default:
                            result.Add(sArray[home + i]);
                            i++;
                            break;
                    }
                }
            }
            

            return;
        }


        //i行目と入れるとfuncArrayに入ってる組を探して対応するendがj行目にくるよ、と返す関数
        public static int nextend(int i)
        {
            if(funcArray.Count != 0)
            {
                for(int tmp = 0;tmp < funcArray.Count;tmp++)
                {
                    Tuple<int,int> t = (Tuple<int,int>)funcArray[tmp];
                    if(t.Item1 == i) return t.Item2;
                }
            }
            return 0;
        }


        //結果がintになる体で作る
        public static void FourOperations(ArrayList sArray,int i)
        {
            //とりあえず数字の計算をさせたい
            string s = (string)sArray[i];
            if(s.Contains(@"a-zA-Z"))
            {
                return;
            }

            //四則演算の式になっていないとうまく使えないので書いている途中は何もしない
            if(s.EndsWith("+") || s.EndsWith("-") || s.EndsWith("*") || s.EndsWith("/") || s.EndsWith("."))
            {
                return;
            }

            //ここで計算
            System.Data.DataTable dt = new System.Data.DataTable();
            //Dictionary<string , string> dict = new Dictionary<string , string>( );
            //dict.Add("x" , "3");
            //string strOriginal = "3*x+1";
            //string str0 = "3*";
            //string str1 = "+1";
            //string str = str0 + dict["x"] + str1;
            int result = (int)dt.Compute(s,"");
            sArray[i] = result.ToString();
        }
        ////FourOperations(string,ref intの変数,ref doubleの変数)で使う(参照渡し)
        //public static void FourOperations(string s , ref int result1 , ref double result2)
        //{
        //    //とりあえず数字の計算をさせたい

        //    if(s.Contains(@"a-zA-Z"))
        //    {
        //        return;
        //    }

        //    //四則演算の式になっていないとうまく使えないので書いている途中は何もしない
        //    if(s.EndsWith("+") || s.EndsWith("-") || s.EndsWith("*") || s.EndsWith("/") || s.EndsWith("."))
        //    {
        //        return;
        //    }

        //    //ここで計算
        //    System.Data.DataTable dt = new System.Data.DataTable( );

        //    //出力するとき型があってないといけないらしいので型をとって条件分岐
        //    Type t = dt.Compute(s , "").GetType( );

        //    //分岐
        //    if(t.ToString( ) == "System.DBNull")
        //    {

        //    }
        //    else {
        //        if(t.ToString( ) == "System.Int32")
        //        {
        //            result1 = (int)dt.Compute(s , "");

        //        }
        //        else
        //        {
        //            result2 = (double)dt.Compute(s , "");
        //        }
        //    }
        //}


        //プロセスを作ろう
        //public static void makeprocess(ProcessfulObject pfo,string s1)
        //{
        //    char[ ] delimiterChars = { ' ' , ',' , '.' , ':' , '\t' , '\n' };

        //    ArrayList sArray = new ArrayList( );
        //    string[ ] s2 = s1.Split(delimiterChars);
        //    for(int i = 0; i < s2.Length; i++)
        //    {
        //        sArray.Add(s2[i]);
        //    }

        //}




        //こっから下で判定してあってるかあってないか出す奴をやりたい

        //文字数のカウントをする
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


        //For()に突っ込んでいいのか判断するためのbool
        public static bool boolfor(ArrayList sArray,int home)
        {
            //一致してるかは知りたいけどうしろに余計なのがついてたらはじきたい
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\(\s*\w+\s*\=\s*\d+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)\s*"))
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\(\s*\w+\s*\=\s*\d+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)\s*."))
                {
                    misfor = true;
                    return false;
                }
                return true;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+\s*=\s*\d+\s*to\s*\d+\s*"))
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+\s*=\s*\d+\s*to\s*\d+\s*."))
                {
                    misfor = true;
                    return false;
                }
                return true;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\d+\s*"))
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\d+\s*."))
                {
                    misfor = true;
                    return false;
                }
                return true;
            }
            misfor = true;
            return false;
        }


        public static bool boolif(ArrayList sArray,int home)
        {
            string[] re = new string[5];
            re[0] = @"[0-9a-zA-Z]+\s*\<\s*[0-9a-zA-Z]+";
            re[1]= @"[0-9a-zA-Z]+\s*\>\s*[0-9a-zA-Z]+";
            re[2] = @"[0-9a-zA-Z]+\s*\<\s*\=\s*[0-9a-zA-Z]+";
            re[3] = @"[0-9a-zA-Z]+\s*\>\s*\=\s*[0-9a-zA-Z]+";
            re[4] = @"[0-9a-zA-Z]+\s*\=\s*\=\s*[0-9a-zA-Z]+";

            for(int i = 0;i < re.Length;i++)
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"if\s*\(" + re[i] + @"\)\s*"))
                {
                    if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"if\s*\(" + re[i] + @"\)\s*."))
                    {
                        misif = true;
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }


        //この辺はifの中に書くとき短くしたいからいる
        public static bool firstfor(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("for")) return true;
            return false;
        }
        public static bool firstif(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("if")) return true;
            return false;
        }
        public static bool firstwhile(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("while")) return true;
            return false;
        }
        public static bool firstend(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("end")) return true;
            return false;
        }
        public static bool firstelse(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("else")) return true;
            return false;
        }

        //ifのための判定群
        public static bool hantei(string s)
        {
            //大小判定したい
            Regex re1 = new Regex(@"(?<sahen>\d+)\s*<\s*(?<uhen>\d+)");
            Match m1 = re1.Match(s);

            Regex re2 = new Regex(@"(?<sahen>\d+)\s*>\s*(?<uhen>\d+)");
            Match m2 = re2.Match(s);

            Regex re3 = new Regex(@"(?<sahen>\d+)\s*<\s*=\s*(?<uhen>\d+)");
            Match m3 = re3.Match(s);

            Regex re4 = new Regex(@"(?<sahen>\d+)\s*>\s*=\s*(?<uhen>\d+)");
            Match m4 = re4.Match(s);

            Regex re5 = new Regex(@"(?<sahen>\d+)\s*=\s*=\s*(?<uhen>\d+)");
            Match m5 = re5.Match(s);

            if(m1.Length > 0)
            {
                int sahen = int.Parse(m1.Groups["sahen"].Value);
                int uhen = int.Parse(m1.Groups["uhen"].Value);
                if(sahen < uhen) return true;
                else return false;
            }
            if(m2.Length > 0)
            {
                int sahen = int.Parse(m2.Groups["sahen"].Value);
                int uhen = int.Parse(m2.Groups["uhen"].Value);
                if(sahen > uhen) return true;
                else return false;
            }
            if(m3.Length > 0)
            {
                int sahen = int.Parse(m3.Groups["sahen"].Value);
                int uhen = int.Parse(m3.Groups["uhen"].Value);
                if(sahen <= uhen) return true;
                else return false;
            }
            if(m4.Length > 0)
            {
                int sahen = int.Parse(m4.Groups["sahen"].Value);
                int uhen = int.Parse(m4.Groups["uhen"].Value);
                if(sahen >= uhen) return true;
                else return false;
            }
            if(m5.Length > 0)
            {
                int sahen = int.Parse(m5.Groups["sahen"].Value);
                int uhen = int.Parse(m5.Groups["uhen"].Value);
                if(sahen == uhen) return true;
                else return false;
            }
            return false;
        }
    }
}
