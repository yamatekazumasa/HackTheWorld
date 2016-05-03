using System;
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
        //forとかifとかのかっこの位置を記録しておきたい
        //エレガントなやり方が思いつかない
        //かっこのセットが配列のいくつめかを知りたい
        const int n = 10;
        public static Vector[ ] forv = new Vector[n];
        static int forcount = 0;
        public static Vector[ ] whilev = new Vector[n];
        static int whilecount = 0;
        public static Vector[ ] ifv = new Vector[n];
        static int ifcount = 0;

        //たぶん再帰的な関数になる
        //かっこが入れ子になってるときとか
        public static void Outside(string[ ] sArray , int state)
        {
            for(int i = 0 + state; i < sArray.Length; i++)
            {
                if(sArray[i] == "for") Separate(sArray , 1 , i);
                if(sArray[i] == "while") Separate(sArray , 2 , i);
                if(sArray[i] == "if") Separate(sArray , 3 , i);
            }
        }
        //わける
        public static void Separate(string[ ] sArray , int type , int _start)
        {
            int i = 0;
            int count = 0;
            for(i = _start; i < sArray.Length; i++)
            {
                if(sArray[i] == "for" || sArray[i] == "if" || sArray[i] == "while") count++;
                if(sArray[i] == "}" && count == 0)
                {
                    //関数の書いてある部分をコピーする
                    string[ ] separatedArray = new string[i - _start];
                    Array.Copy(sArray , _start , separatedArray , 0 , i - _start);
                    //始まりと終わりをそれぞれ記録する
                    switch(type)
                    {
                        case 1:
                            forv[forcount].X = _start;
                            forv[forcount].Y = i;
                            forcount++;
                            break;
                        case 2:
                            whilev[whilecount].X = _start;
                            whilev[whilecount].Y = i;
                            whilecount++;
                            break;
                        case 3:
                            ifv[ifcount].X = _start;
                            ifv[ifcount].Y = i;
                            ifcount++;
                            break;
                    }
                    //複数の式が入ってるときは分割されてほしい
                    if(countfunction(separatedArray) != 1)
                    {
                        Outside(separatedArray , 1);
                    }
                    if(separatedArray[0] == "for") For(separatedArray);

                    break;
                }
            }


        }
        public static void For(string[ ] sArray)
        {
            
        }
        //FourOperations(string,ref intの変数,ref doubleの変数)で使う(参照渡し)
        public static void FourOperations(string s,ref int result1,ref double result2)
        {
            //とりあえず数字の計算をさせたい

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

            //出力するとき型があってないといけないらしいので型をとって条件分岐
            Type t = dt.Compute(s , "").GetType( );

            //分岐
            if(t.ToString( ) == "System.DBNull")
            {

            }
            else {
                if(t.ToString( ) == "System.Int32")
                {
                    result1 = (int)dt.Compute(s , "");
                    
                }
                else
                {
                    result2 = (double)dt.Compute(s , "");
                }
            }
        }

        //private void Wait( )
        //{
        //      string test = textBox1.Text;
        //    string [] testdata =test.Split(' ');
        //}
        //的な感じで他のも作りたい


        //式がいくつ入ってるかを出す関数
        public static int countfunction(string[ ] sArray)
        {
            int count = 0;
            for(int i = 0; i < sArray.Length; i++)
            {
                if(sArray[i] == "for" || sArray[i] == "while" || sArray[i] == "if") count++;
            }
            return count;
        }

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
        //中かっこと閉じかっこの数が同じかどうか
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
