using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;


namespace HackTheWorld
{
    public static class Kakikae
    {
        //forとかifとかのかっこの組が配列の何番目か記録しておきたい
        //いいやり方が思いつかない

        //const int constn = 10;
        //public static Vector[ ] kakkoset = new Vector[constn];
        //static int kakkocount = 0;

        //arraylistのなかにvectorをもたせる
       public static ArrayList forArray=new ArrayList();
        public static ArrayList ifArray=new ArrayList();
        //個別に作るけど(インデントのため)まとまってたほうが何かと便利
        public static ArrayList funcArray=new ArrayList();
        static bool mismatch = false;
        public static string yomitori(string s1)
        {
            //連続で入力してデバックしたいからいる奴ら
            forArray.Clear( );
            ifArray.Clear( );
            funcArray.Clear( );

            char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };

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
                str += (string)result[i]+"\n";
                Tuple<int , int> t = Tuple.Create<int , int>(1, 2);
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
                if((string)sArray[i] == "for" || (string)sArray[i] == "if") countfunction++;
            }
            //初めのほうから順番に見ていく
            for(int i = 0; i < sArray.Count; i++)
            {
                if((string)sArray[i] == "for")
                {
                    kakko++;
                    for(int j = i + 1; j < sArray.Count; j++)
                    {
                        if((string)sArray[j] == "for" || (string)sArray[i] == "if") kakko++;
                        if((string)sArray[j] == "endfor") kakko--;
                        if(kakko == 0)
                        {
                            forArray.Add(new Vector(i , j));
                            //forArray.Add(Tuple.Create(i, j));
                            funcArray.Add(new Vector(i , j));
                            countfunction--;
                            break;
                        }
                    }
                    mismatch = true;
                }
                if((string)sArray[i] == "if")
                {
                    kakko++;
                    for(int j = i + 1; j < sArray.Count; j++)
                    {
                        if((string)sArray[j] == "for" || (string)sArray[i] == "if") kakko++;
                        if((string)sArray[j] == "endif") kakko--;
                        if(kakko == 0)
                        {
                            ifArray.Add(new Vector(i , j));
                            funcArray.Add(new Vector(i , j));
                            countfunction--;
                            break;
                        }
                    }
                    mismatch = true;
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
                //funcArrayのvectorのXとYの中にいないときは素直にスルー
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
                            Vector tmpi = (Vector)funcArray[i];
                            if((string)sArray[(int)tmpi.X] == "for")
                            {
                                //コピー
                                //ぶっちゃけcopyの関数のがいい説ある
                                ArrayList forlist = new ArrayList( );
                                for(int j = (int)tmpi.X; j <= (int)tmpi.Y; j++)
                                {
                                    forlist.Add(sArray[j]);
                                }
                                if(boolfor(forlist))
                                {
                                    //Forに入れる
                                    for(int j = 0; j < For(forlist).Count; j++)
                                    {
                                        result.Add(For(forlist)[j]);
                                      
                                    }
                                    k += (int)tmpi.Y - (int)tmpi.X+1;
                                    i++;
                                    break;
                                }
                            }
                            //if((string)sArray[(int)tmpi.X] == "if")
                            //{
                            //    //コピー
                            //    ArrayList iflist = new ArrayList( );
                            //    for(int j = (int)tmpi.X; j <= (int)tmpi.Y; j++)
                            //    {
                            //        iflist.Add(sArray[j]);
                            //    }

                            //    //Forに入れる
                            //    for(int j = 0; j < If(iflist).Count; j++)
                            //    {
                            //        result.Add(If(iflist)[j]);
                            //        k += (int)tmpi.Y - (int)tmpi.X + 1;
                            //    }

                            //}

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
            Vector tmpi = (Vector)funcArray[i];
                for(int j = 0; j < funcArray.Count; j++)
                {
                    Vector tmpj = (Vector)funcArray[j];
                    if(tmpj.X < tmpi.X && tmpj.Y > tmpi.Y) return true;
                }
            
            return false;
        }
        //iがfuncArrayの中のvectorのXとYに挟まれてたらtrue
        public static bool intinside(int i)
        {
            if(funcArray != null)
            {
                for(int j = 0; j < funcArray.Count; j++)
                {
                    Vector tmpj = (Vector)funcArray[j];
                    if((int)tmpj.Y != 0)
                    {
                        if(tmpj.X <= i && tmpj.Y >= i) return true;
                    }
                }
            }
            return false;
        }
        public static ArrayList For(ArrayList sArray)
        {
            //for(sArray[0])の次は繰り返し回数としている
            int n = int.Parse((string)sArray[1]);
            ArrayList expansion = new ArrayList( );
            ArrayList insidefor = new ArrayList( );
            for(int i = 0; i < n; i++)
            {
                //[2]から繰り返し
                for(int j = 2; j < sArray.Count; j++)
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

                    if((string)sArray[j] == "endfor") break;
                    expansion.Add(sArray[j]);

                }
            }
            return expansion;
        }
        //For()に突っ込んでいいのか判断するためのbool
        public static bool boolfor(ArrayList sArray)
        {
            //[1]が数字かどうか
            int inttest = 0;
            //[1]がintじゃないなら四則演算のほうに回す

            if(!sArray.Contains("for") || !sArray.Contains("endfor")) return false;
            if(!int.TryParse((string)sArray[1] , out inttest)) return false;
            return true;
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
