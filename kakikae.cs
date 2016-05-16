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
    public static class Kakikae
    {
        //forとかifとかのかっこの組が配列の何番目か記録しておきたい
        //いいやり方が思いつかない

        //const int constn = 10;
        //public static Vector[ ] kakkoset = new Vector[constn];
        //static int kakkocount = 0;

        //arraylistのなかにTuple<int,int>をもたせる
        public static ArrayList forArray = new ArrayList( );
        public static ArrayList ifArray = new ArrayList( );
        //個別に作るけど(インデントのため)まとまってたほうが何かと便利
        public static ArrayList funcArray = new ArrayList( );
        static bool mismatch = false;
        static bool misfor = false;
        public static string yomitori(string s1)
        {
            //連続で入力してデバックしたいからいる奴ら
            forArray.Clear( );
            ifArray.Clear( );
            funcArray.Clear( );

            //char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };
            char[ ] delimiterChars = { '\n' };

            ArrayList sArray = new ArrayList( );
            ArrayList result = new ArrayList( );
            string str = "";

            string[ ] s2 = s1.Split(delimiterChars);
            for(int i = 0; i < s2.Length; i++)
            {
                sArray.Add(s2[i]);
            }
            kakkoread(sArray);
            if(mismatch)
            {
                MessageBox.Show("関数の始まりと終わりの対応が乱れてる");
                return str;
            }
            warifuri(sArray , result);
            if(misfor)
            {
                MessageBox.Show("forとendforはいるみたいだけど\n文の中身が違う");
                return str;
            }
            for(int i = 0; i < result.Count; i++)
            {
                str += (string)result[i] + "\n";
                //Tuple<int , int> t = Tuple.Create<int , int>(1 , 2);
            }
            MessageBox.Show(str);
            return str;
        }
        //かっこの組を出す
        public static void kakkoread(ArrayList sArray)
        {
            int countfunction = 0;
            int kakko = 0;
            for(int i = 0; i < sArray.Count; i++)
            {

                //関数の数を数える(今はforとifだけ)
                if(sArray[i].ToString( ).StartsWith("for") || sArray[i].ToString( ).StartsWith("if")) countfunction++;
            }
            //初めのほうから順番に見ていく
            for(int i = 0; i < sArray.Count; i++)
            {
                if(sArray[i].ToString( ).StartsWith("for"))
                {
                    kakko++;
                    for(int j = i + 1; j < sArray.Count; j++)
                    {
                        if(sArray[j].ToString( ).StartsWith("for") || sArray[j].ToString( ).StartsWith("if")) kakko++;
                        if(sArray[j].ToString( ).StartsWith("endfor")) kakko--;
                        if(kakko == 0)
                        {
                            forArray.Add(new Tuple<int , int>(i , j));
                            funcArray.Add(new Tuple<int , int>(i , j));
                            countfunction--;
                            break;
                        }
                    }
                    if(countfunction != 0) mismatch = true;
                }
                if(sArray[i].ToString( ).StartsWith("if"))
                {
                    kakko++;
                    for(int j = i + 1; j < sArray.Count; j++)
                    {
                        if(sArray[j].ToString( ).StartsWith("for") || sArray[j].ToString( ).StartsWith("if")) kakko++;
                        if(sArray[j].ToString( ).StartsWith("endif")) kakko--;
                        if(kakko == 0)
                        {
                            ifArray.Add(new Tuple<int , int>(i , j));
                            funcArray.Add(new Tuple<int , int>(i , j));
                            countfunction--;
                            break;
                        }
                    }
                    if(countfunction != 0) mismatch = true;
                }
                if(countfunction == 0) break;
            }
        }

        //そうやって得られたfunctionの位置から各々の関数にとびたい
        //他の関数が増えたら場合分けが増えるだろう
        public static void warifuri(ArrayList sArray , ArrayList result)
        {
            int i = 0;
            int k = 0;
            while(k < sArray.Count)
            {
                //funcArrayのTupleのitem1とitem2の中にいないときは素直にスルー
                if(!intinside(k))
                {
                    result.Add(sArray[k]);
                    k++;
                    continue;
                }
                else
                {
                    while(true)
                    {
                        //関数のの中か否か
                        if(!kakkoinside(i))
                        {
                            Tuple<int , int> tmpi = (Tuple<int , int>)funcArray[i];
                            if(sArray[tmpi.Item1].ToString( ).StartsWith("for"))
                            {
                                //コピー
                                ArrayList forlist = new ArrayList( );
                                for(int j = tmpi.Item1; j <= tmpi.Item2; j++)
                                {
                                    forlist.Add(sArray[j]);
                                }
                                //Forに入れる
                                for(int j = 0; j < For(forlist).Count; j++)
                                {
                                    result.Add(For(forlist)[j]);
                                }
                                k += tmpi.Item2 - tmpi.Item1 + 1;
                                i++;
                                break;

                            }
                            if(sArray[tmpi.Item1].ToString( ).StartsWith("if"))
                            {
                                //コピー
                                ArrayList iflist = new ArrayList( );
                                for(int j = tmpi.Item1; j <= tmpi.Item2; j++)
                                {
                                    iflist.Add(sArray[j]);
                                }
                                //Ifに入れる
                                for(int j = 0; j < If(iflist).Count; j++)
                                {
                                    result.Add(If(iflist)[j]);
                                }
                                k += tmpi.Item2 - tmpi.Item1 + 1;
                                i++;
                                break;

                            }

                        }
                    }
                }

            }
        }

        /* for文について
         * 
         * for 2
         * function(3*i)　 ←function(0) function(3)を実行
         * endfor 
         * ↑こんな感じで
         * 
         * for i = 1 to 3
         * 
         * for(i=0;i<5;i++)
         * 
         * if文について
         * 
         * if(条件式){
         * 　なんか;
         * } else if(条件式){
         * 　何かしら;
         * }
         * やっぱelseいる
         * 正規表現
         * 
         * if i == 0 do
         * k = 2 
         * endif
         * 
         * if i == 1 do
         * k = 3
         * endif
         * 
         * if i!=0 do 
         *   if i!=1 do
         *   k = 0
         *   endif
         * endif
         * このパターンだと「if - endif」の構造を作るだけで完了
         * 分かりやすい 
         * 
         * スペース区切りの問題があるので、最初は厳密なスペースの文法に合わせた認識をさせる
         * if i == 0 do
         *   function
         * endif   
         * 
         * ↓これはまだ良い
         * if i==0 do
         *   function
         * endif
         * 
         * 
         * while文について(これちょっと保留)
         * 
         * while(buttonClicked() == true){  ←条件式中身はif文を流用できそう
         *   function();
         * }
         * 
         * while  do
         *   function
         * endwhile
         * 
         * 
         * forを発見したらendforまでをforとする
         * 
         * ボタンを押すとコードを読み取る(GetString to ArrayList)
         * 
         * ①左辺 = 右辺 の左辺が未定義だった場合、左辺を読み取る(ArrayList -> extracting various)
         * ②for if while 追加していく(ArrayList -> ArrayList・改)
         * ここまででProcessにしやすい文構造になっててほしい
         * ----変換の壁----
         * ③Process作る人 (ArrayList・改 -> Process) ←variousをいちいちチェック(無ければエラー)
         * ----出力の壁----
         */




        //funcArray[i]が何かの内側ならtrueを返す
        //これがないと内側が2回実行される
        public static bool kakkoinside(int i)
        {
            Tuple<int , int> tmpi = (Tuple<int , int>)funcArray[i];
            for(int j = 0; j < funcArray.Count; j++)
            {
                Tuple<int , int> tmpj = (Tuple<int , int>)funcArray[j];
                if(tmpj.Item1 < tmpi.Item1 && tmpj.Item2 > tmpi.Item2) return true;
            }

            return false;
        }
        //iがfuncArrayの中のTupleの中身に挟まれてたらtrue
        public static bool intinside(int i)
        {
            if(funcArray != null)
            {
                for(int j = 0; j < funcArray.Count; j++)
                {
                    Tuple<int , int> tmpj = (Tuple<int , int>)funcArray[j];
                    if(tmpj.Item2 != 0)
                    {
                        if(tmpj.Item1 <= i && tmpj.Item2 >= i) return true;
                    }
                }
            }
            return false;
        }

        //For()に突っ込んでいいのか判断するためのbool
        public static bool boolfor(ArrayList sArray)
        {
            //一致してるかは知りたいけど余計なのがついてたらはじきたい
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\(\s*\w+\s*\=\s*\d+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)\s*"))
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\(\s*\w+\s*\=\s*\d+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)\s*."))
                {
                    misfor = true;
                    return false;
                }
                return true;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\w+\s*=\s*\d+\s*to\s*\d+\s*"))
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\w+\s*=\s*\d+\s*to\s*\d+\s*."))
                {
                    misfor = true;
                    return false;
                }
                return true;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\d+\s*"))
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\d+\s*."))
                {
                    misfor = true;
                    return false;
                }
                return true;
            }
            misfor = true;
            return false;
        }
        public static ArrayList For(ArrayList sArray)
        {
            if(!boolfor(sArray))
            {
                return sArray;
            }
            int type = 0;
            //sArray[0]はforから始まっていて繰り返し回数を指定している行
            //どんな書き方をしているかの正規表現を用いた場合分けをしたい
            //いちいち間に\s*(0個以上の空白文字を示す)を入れて間に空白が入っても読めるようにする
            //typeを3つ作ることにする

            //for(i=0;i<5;i++)がtype1
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\(\s*\w+\s*\=\s*\d+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)")) type = 1;
            //for i=0 to 3がtype2
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\w+\s*=\s*\d+\s*to\s*\d+")) type = 2;
            //for 2とかをtype3とする
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"for\s*\d+")) type = 3;


            ArrayList expansion = new ArrayList( );
            ArrayList insidefor = new ArrayList( );

            //これifで使うような条件式判定が必要そう(あとまわし)
            //if(type == 1)
            //{
            //    Regex re1 = new Regex(@"for\s*\(\s*(?<hensuu>\w+)\s*\=\s*(?<start>\d+)\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\d+\s*;\s*\w+[\+\+|\-\-|\+=\d+|\-=\d+]\)");
            //    Match m1 = re1.Match((string)sArray[0]);
            //    int n = int.Parse(m1.Groups["end"].Value) - int.Parse(m1.Groups["start"].Value) + 1;

            //    for(int i = 0; i < n; i++)
            //    {
            //        for(int j = 1; j < sArray.Count; j++)
            //        {
            //            if(sArray[j].ToString( ).StartsWith("for"))
            //            {
            //                for(int k = j; k < sArray.Count; k++)
            //                {
            //                    insidefor.Add(sArray[k]);
            //                }
            //                for(int k = 0; k < For(insidefor).Count; k++)
            //                {
            //                    expansion.Add(For(insidefor)[k]);
            //                }
            //                break;
            //            }

            //            if(sArray[j].ToString( ).StartsWith("endfor")) break;
            //            expansion.Add(sArray[j]);

            //        }
            //    }
            //    return expansion;
            //}

            if(type == 2)
            {
                Regex re2 = new Regex(@"for\s*(?<hensuu>\w+)\s*=\s*(?<start>\d+)\s*to\s*(?<end>\d+)");
                Match m2 = re2.Match((string)sArray[0]);
                int n = int.Parse(m2.Groups["end"].Value) - int.Parse(m2.Groups["start"].Value) + 1;

                for(int i = 0; i < n; i++)
                {
                    for(int j = 1; j < sArray.Count; j++)
                    {
                        if(sArray[j].ToString( ).StartsWith("for"))
                        {
                            for(int k = j; k < sArray.Count; k++)
                            {
                                insidefor.Add(sArray[k]);
                            }
                            for(int k = 0; k < For(insidefor).Count; k++)
                            {
                                expansion.Add(For(insidefor)[k]);
                            }
                            break;
                        }

                        if(sArray[j].ToString( ).StartsWith("endfor")) break;
                        expansion.Add(sArray[j]);

                    }
                }
                return expansion;
            }


            if(type == 3)
            {
                Regex re3 = new Regex(@"for\s*(?<repeat>\d+)");
                Match m3 = re3.Match((string)sArray[0]);
                int n = int.Parse(m3.Groups["repeat"].Value);

                for(int i = 0; i < n; i++)
                {
                    //[1]から繰り返し
                    for(int j = 1; j < sArray.Count; j++)
                    {
                        if(sArray[j].ToString( ).StartsWith("for"))
                        {
                            for(int k = j; k < sArray.Count; k++)
                            {
                                insidefor.Add(sArray[k]);
                            }
                            for(int k = 0; k < For(insidefor).Count; k++)
                            {
                                expansion.Add(For(insidefor)[k]);
                            }
                            break;
                        }

                        if(sArray[j].ToString( ).StartsWith("endfor")) break;
                        expansion.Add(sArray[j]);

                    }
                }
                return expansion;
            }
            expansion.Add("ここのforうまくいってない");
            return expansion;
        }
        public static bool boolif(ArrayList sArray)
        {
            Regex re = new Regex(@"if\s*(?<hensuu>\w+)\s*");
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[0] , @"if\s*(?<hensuu>\w+)\s*"))
            {
                //Match m = re.Match((string)sArray[0]);
                //string hensuu = m.Groups["hensuu"].Value.ToString( );

                return true;
            }
            return false;
        }
        public static bool hantei(string s)
        {
            return false;
        }
        public static ArrayList If(ArrayList sArray)
        {
            ArrayList expantion = new ArrayList( );

            return expantion;
        }
        //結果がintになる体で作る
        public static void FourOperations(ArrayList sArray , int i)
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
            System.Data.DataTable dt = new System.Data.DataTable( );
            //Dictionary<string , string> dict = new Dictionary<string , string>( );
            //dict.Add("x" , "3");
            //string strOriginal = "3*x+1";
            //string str0 = "3*";
            //string str1 = "+1";
            //string str = str0 + dict["x"] + str1;
            int result = (int)dt.Compute(s , "");
            sArray[i] = result.ToString( );
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

        //private void Wait( )
        //{
        //      string test = textBox1.Text;
        //    string [] testdata =test.Split(' ');
        //}
        //的な感じで他のも作りたい

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
        //文字のカウント
        public static int CountChar(string s , char c)
        {
            return s.Length - s.Replace(c.ToString( ) , "").Length;
        }
        //ノーマルかっこと閉じかっこ、中かっこと中閉じかっこの数が同じかどうか(いらない気がしてきた)
        public static bool kakkocounter(ArrayList sArray)
        {
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0;
            for(int i = 0; i < sArray.Count; i++)
            {
                count1 += CountChar(sArray[i].ToString( ) , '(');
                count2 += CountChar(sArray[i].ToString( ) , ')');
                count3 += CountChar(sArray[i].ToString( ) , '{');
                count4 += CountChar(sArray[i].ToString( ) , '}');
            }
            if(count1 == count2 && count3 == count4) return true;
            return false;
        }





    }
}
