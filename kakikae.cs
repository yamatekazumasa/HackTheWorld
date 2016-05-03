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
        public static void Outside(string[ ] sArray)
        {
            for(int i = 0; i < sArray.Length; i++)
            {
                if(sArray[i] == null) continue;
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
                    if(countfunction(separatedArray) != 1)
                    {
                        Outside(separatedArray);
                    }
                    if(separatedArray[0] == "for") For(separatedArray);

                    break;
                }
            }
            
            
        }
        public static void For(string[] sArray)
        {

        }
        //public static void FourOperations(string test)
        //{
        //    //とりあえず数字の計算をさせたい

        //    //string test = _box.GetString( );
        //    string[ ] testdata = test.Split(' ');

        //    if(test.Contains(@"a-zA-Z"))
        //    {
        //        return;
        //    }

        //    //四則演算の式になっていないとうまく使えないので書いている途中は何もしない
        //    if(test.EndsWith("+") || test.EndsWith("-") || test.EndsWith("*") || test.EndsWith("/") || test.EndsWith("."))
        //    {
        //        return;
        //    }

        //    //ここで計算
        //    System.Data.DataTable dt = new System.Data.DataTable( );


        //    //出力するとき型があってないといけないらしいので型をとって条件分岐
        //    Type t = dt.Compute(test , "").GetType( );



        //    //分岐
        //    if(t.ToString( ) == "System.DBNull")
        //    {
        //        GraphicsContext.DrawString("aiueo" , new Font("Arial" , 12) , Brushes.Black , new Rectangle(500 , 300 , 500 , 300));


        //    }
        //    else {
        //        if(t.ToString( ) == "System.Int32")
        //        {
        //            int result = (int)dt.Compute(test , "");
        //            GraphicsContext.DrawString(result.ToString( ) , new Font("Arial" , 12) , Brushes.Black , new Rectangle(500 , 300 , 500 , 300));
        //        }
        //        else
        //        {
        //            double result = (double)dt.Compute(test , "");
        //            GraphicsContext.DrawString(result.ToString( ) , new Font("Arial" , 12) , Brushes.Black , new Rectangle(500 , 300 , 500 , 300));
        //        }
        //    }
        //}

        //private void Wait( )
        //{
        //      string test = textBox1.Text;
        //    string [] testdata =test.Split(' ');
        //}
        //的な感じで他のも作りたい


            //式がいくつ入ってるかを出す関数
            public static int countfunction(string[] sArray)
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
