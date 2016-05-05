using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;


namespace HackTheWorld
{
    public static class Kakikae
    {
        //forとかifとかのかっこの組が配列の何番目か記録しておきたい
        //いいやり方が思いつかない
     
        const int constn = 10;
        public static Vector[ ] kakkoset = new Vector[constn];
        static int kakkocount = 0;


        public static string yomitori(string s1)
        {
            char[ ] delimiterChars = { ' ' , ',' , '.' , ':' , '\t' , '\n' };

            ArrayList sArray = new ArrayList( );
            ArrayList result = new ArrayList( );
            string[ ] s2 = s1.Split(delimiterChars);
            for(int i = 0; i < s2.Length; i++)
            {
                sArray.Add(s2[i]);
            }
            kakkoread(sArray);
            warifuri(sArray , result);
            string str = "";
            for(int i = 0; i < result.Count; i++)
            {
                str += (string)result[i] + "\n";
            }
            return str;
        }
        //かっこの組を出す
        public static void kakkoread(ArrayList sArray)
        {
            int countfunction = 0;
            int kakko = 0;
            for(int i = 0; i < sArray.Count; i++)
            {
                //関数の数を数える(今はforだけ)
                if((string)sArray[i] == "for") countfunction++;
            }
            //初めのほうから順番に見ていく
            for(int i = 0; i < sArray.Count; i++)
            {
                if((string)sArray[i] == "{")
                {
                    kakkoset[kakkocount].X = i;
                    kakko++;
                    for(int j = i + 1; j < sArray.Count; j++)
                    {
                        if((string)sArray[j] == "{") kakko++;
                        if((string)sArray[j] == "}") kakko--;
                        if(kakko == 0)
                        {
                            kakkoset[kakkocount].Y = j;
                            kakkocount++;
                            countfunction--;
                            break;
                        }
                    }
                }
                if(countfunction == 0) break;
            }
        }

        //そうやって得られたかっこの組から関数に飛びたい
        //forに関してはfor N{なんとかかんとか}の形を考えているので、{から二つ前の部分を見る
        //他の関数が増えたら場合分けがつくだろう
        public static void warifuri(ArrayList sArray , ArrayList result)
        {
            int k = 0;
            while( k < sArray.Count)
            {
                //forから閉じかっこの中にいないときは素直にスルー
                if(!intinside(k))
                {
                    result.Add(sArray[k]);
                    k++;
                    continue;
                }
                else {
                    for(int i = 0; i < constn; i++)
                    {
                        if(!kakkoinside(i))
                        {
                            if((int)kakkoset[i].X - 2 >= 0)
                            {
                                if((string)sArray[(int)kakkoset[i].X - 2] == "for")
                                {
                                    //コピー
                                    ArrayList forlist = new ArrayList( );
                                    for(int j = (int)kakkoset[i].X - 2; j <= (int)kakkoset[i].Y; j++)
                                    {
                                        forlist.Add(sArray[j]);
                                    }

                                    //Forに入れる
                                    for(int j = 0; j < For(forlist).Count; j++)
                                    {
                                        result.Add(For(forlist)[j]);
                                        k += (int)kakkoset[i].Y - (int)kakkoset[i].X+1;
                                    }

                                }
                            }
                        }
                    }
                }

            }
        }
        //kakkoset[i]が何かの内側ならtrueを返す
        //これがないと内側が2回実行される
        public static bool kakkoinside(int i)
        {
            for(int j = 0; j < constn; j++)
            {
                if(kakkoset[j].X < kakkoset[i].X && kakkoset[j].Y > kakkoset[i].Y) return true;
            }
            return false;
        }
        //iがkakkoset[].XとYに挟まれてたらtrue
        public static bool intinside(int i)
        {
            for(int j = 0; j < constn; j++)
            {
                if((int)kakkoset[j].X != 0 && (int)kakkoset[j].Y != 0)
                {
                    if(kakkoset[j].X - 2 <= i && kakkoset[j].Y >= i) return true;
                }
            }
            return false;
        }
        public static ArrayList For(ArrayList sArray)
        {
            //[1]が数字かどうか
            int inttest = 0;
            if(!int.TryParse((string)sArray[1] , out inttest)) FourOperations(sArray , 1);

            //for(sArray[0])の次は繰り返し回数としている
            int n = int.Parse((string)sArray[1]);
            ArrayList expansion = new ArrayList( );
            ArrayList insidefor = new ArrayList( );
            for(int i = 0; i < n; i++)
            {


                //[3]からかっこの中
                for(int j = 3; j < sArray.Count; j++)
                {
                    if((string)sArray[j] == "for")
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

                    if((string)sArray[j] == "}") break;
                    expansion.Add(sArray[j]);

                }
            }
            return expansion;
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
        //ノーマルかっこと閉じかっこの数が同じかどうか(いらない気がしてきた)
        public static bool counterN(string[ ] sArray)
        {
            int count1 = 0, count2 = 0;
            for(int i = 0; i < sArray.Length; i++)
            {
                if(sArray[i] == "(") count1++;
                if(sArray[i] == ")") count2++;
            }
            if(count1 == count2) return true;
            else return false;
        }
        //中かっこと閉じかっこの数が同じかどうか(ついで)
        public static bool counterM(string[ ] sArray)
        {
            int count1 = 0, count2 = 0;
            for(int i = 0; i < sArray.Length; i++)
            {
                if(sArray[i] == "{") count1++;
                if(sArray[i] == "}") count2++;
            }
            if(count1 == count2) return true;
            else return false;
        }




    }
}
