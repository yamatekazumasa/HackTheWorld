using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameObject
    {
        private int _x;
        private int _y;
        private int _vx;
        private int _vy;
        private int _w;
        private int _h;

        /// <summary>
        /// 生死フラグ。基本はdelete以外で弄らないように。
        /// </summary>
        public bool _isAlive;
        /// <summary>
        /// オブジェクトのタイプ。enemy、player、bullet、itemなど。
        /// </summary>
        private ObjectType _objectType;

        #region コンストラクタ

        /// <summary>
        /// 何も指定せずにオブジェクトを出現させる。
        /// </summary>
        public GameObject()
        {
            Initialize();
        }

        /// <summary>
        ///  位置を指定してオブジェクトを出現させる。
        /// </summary>
        /// <param name="x">初期x座標。</param>
        /// <param name="y">初期y座標。</param>
        public GameObject(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 位置と速度を指定してオブジェクトを出現させる。
        /// </summary>
        /// <param name="x">初期x座標。</param>
        /// <param name="y">初期y座標。</param>
        /// <param name="vx">初期速度のx方向成分。</param>
        /// <param name="vy">初期速度のy方向成分。</param>
        public GameObject(int x, int y, int vx, int vy) : this(x, y)
        {
            VX = vx;
            VY = vy;
        }

        /// <summary>
        /// 位置と速度とサイズを指定してオブジェクトを出現させる。
        /// </summary>
        /// <param name="x">初期x座標。</param>
        /// <param name="y">初期y座標。</param>
        /// <param name="vx">初期速度のx方向成分。</param>
        /// <param name="vy">初期速度のy方向成分。</param>
        /// <param name="w">幅。</param>
        /// <param name="h">高さ。</param>
        public GameObject(int x, int y, int vx, int vy, int w, int h) : this(x, y, vx, vy)
        {
            Width = w;
            Height = h;
        }

        #endregion


        #region アクセサ

        public Vector Position
        {
            get { return new Vector(_x, _y) / Scale; }
            set
            {
                _x = (int)(value.X * Scale);
                _y = (int)(value.Y * Scale);
            }
        }

        public Vector Velocity
        {
            get { return new Vector(_vx, _vy) / Scale; }
            set
            {
                _vx = (int)(value.X * Scale);
                _vy = (int)(value.Y * Scale);
            }
        }

        public Vector Size
        {
            get { return new Vector(_w, _h) / Scale; }
            set
            {
                _w = (int)(value.X * Scale);
                _h = (int)(value.Y * Scale);
            }
        }

        public float MinX
        {
            get { return (float)_x / Scale; }
            set { _x = (int)(value*Scale); }
        }

        public float MinY
        {
            get { return (float)_y / Scale; }
            set { _y = (int)(value * Scale); }
        }

        public float MaxX
        {
            get { return (float)(_x + _w) / Scale; }
            set { _x = (int)(value*Scale) - _w; }
        }

        public float MaxY
        {
            get { return (float)(_y + _h) / Scale; }
            set { _y = (int)(value * Scale) - _h; }
        }

        public float MidX
        {
            get { return (float)(_x + _w/2) / Scale; }
            set { _x = (int)(value * Scale) - _w/2; }
        }

        public float MidY
        {
            get { return (float)(_y + _h/2) / Scale; }
            set { _y = (int)(value * Scale) - _h/2; }
        }

        public float X
        {
            get { return MinX; }
            set { MinX = value; }
        }

        public float Y
        {
            get { return MinY; }
            set { MinY = value; }
        }

        public float VX
        {
            get { return (float)_vx / Scale; }
            set { _vx = (int)(value * Scale); }
        }

        public float VY
        {
            get { return (float)_vy / Scale; }
            set { _vy = (int)(value * Scale); }
        }

        public float Width
        {
            get { return (float)_w / Scale; }
            set { _w = (int)(value * Scale); }
        }

        public float Height
        {
            get { return (float)_h / Scale; }
            set { _h = (int)(value * Scale); }
        }

        public float W
        {
            get { return Width; }
            set { Width = value; }
        }

        public float H
        {
            get { return Height; }
            set { Height = value; }
        }

        public ObjectType ObjectType => _objectType;

        public bool Clicked => Contains(Input.Mouse.Position) && Input.LeftButton.Pushed;


        /// <summary>
        /// 自分の矩形範囲を暗黙的に指定できる。
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator Rectangle(GameObject obj)
        {
            return new Rectangle((int)obj.X, (int)obj.Y, (int)obj.W, (int)obj.H);
        }

        /// <summary>
        /// 自分の矩形範囲を暗黙的に指定できる。
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator RectangleF(GameObject obj)
        {
            return new RectangleF(obj.X, obj.Y, obj.W, obj.H);
        }


        /// <summary>
        /// オブジェクトを消す。
        /// </summary>
        public virtual void Die()
        {
            _isAlive = false;
        }

        #endregion


        /// <summary>
        /// 初期化用。
        /// </summary>
        public void Initialize()
        {
            _isAlive = true;
            Size = new Vector(CellSize, CellSize);
        }

        #region GameObject専用

        /// <summary>
        /// 設定された速度で1フレーム分動く。
        /// </summary>
        public virtual void Move(float dt)
        {
            Position += Velocity * dt;
        }
        
        /// <summary>
        /// 角度を指定して、velocityを回転させる。
        /// </summary>
        /// <param name="deg">回転角。degreeで指定する。</param>
        /// <returns></returns>
        public virtual void Rotate(double deg)
        {
            this.Velocity = this.Velocity.Rotate(deg);
        }

        /// <summary>
        /// 加速度を指定して、velocityに加える。
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public virtual void Accelerate(double a)
        {
            this.Velocity = this.Velocity.Extend(a);
        }
        
        /// <summary>
        /// 重なりの判定。
        /// 渡されたオブジェクトの矩形領域と重なっているか判定する。
        /// </summary>
        /// <param name="obj">渡されたオブジェクト。</param>
        /// <returns>重なっていたらtrue、重なっていなかったらfalseを返す。</returns>
        public virtual bool Intersects(GameObject obj)
        {
            return MinX < obj.MaxX && MaxX > obj.MinX &&
                   MinY < obj.MaxY && MaxY > obj.MinY;
        }

        /// <summary>
        /// 包含判定。
        /// 渡されたオブジェクトの矩形領域を包含しているか判定する。
        /// </summary>
        /// <param name="obj">渡されたオブジェクト。</param>
        /// <returns>包含していたらtrue、包含していなかったらfalseを返す。</returns>
        public virtual bool Contains(GameObject obj)
        {
            return MinX < obj.MinX && MaxX > obj.MaxX &&
                   MinY < obj.MinY && MaxY > obj.MaxY;
        }

        /// <summary>
        /// 包含判定。
        /// 渡された点を包含しているか判定する。
        /// </summary>
        public virtual bool Contains(Point p)
        {
            return MinX < p.X && MaxX > p.X && MinY < p.Y && MaxY > p.Y; 
        }
  
        /// <summary>
        /// 包含判定。
        /// 渡された点を包含しているか判定する。
        /// </summary>
        public virtual bool Contains(int x, int y)
        {
            return MinX < x && MaxX > x && MinY < y && MaxY > y;
        }


        /// <summary>
        /// 衝突判定。
        /// </summary>
        /// <param name="obj">渡されたオブジェクトと衝突しているか判定する。</param>
        /// <returns>衝突していたらtrue、衝突していなかったらfalseを返す。</returns>
        public virtual bool CollidesWith(GameObject obj)
        {
            if (!obj._isAlive) return false;
            if (_objectType == obj.ObjectType) return false;
            return Intersects(obj);
        }

        /// <summary>
        /// オブジェクトがウィンドウの中に納まっているか判定する。
        /// </summary>
        /// <returns>オブジェクトがウィンドウ内にあればture、ウインドウ外にあればfalseを返す。</returns>
        public virtual bool InWindow()
        {
            return MinX > -100 && MinX < ScreenWidth + 100 &&
                   MinY > -100 && MinY < ScreenHeight + 100;
        }

        public virtual bool OnGround()
        {
            return true;
        }

        /// <summary>
        /// 衝突後の調整関数。
        /// </summary>
        /// <param name="obj">渡されたオブジェクトに衝突しているとき重ならない状態にする。</param>
        /// <returns>重なっていたらオブジェクトを動かす。</returns>
        public virtual void Adjust(GameObject obj)
        {
            if (Intersects(obj))
            {
                int max = 10;// めり込み許容量。10という値は仮で、要調整。
                if (MaxY > obj.MinY && MaxY - obj.MinY <= max)
                {
                    this.Y -= MaxY - obj.MinY;
                }
                else if (MinY < obj.MaxY && MinY - obj.MaxY >= -max)
                {
                    this.Y -= MinY - obj.MaxY;
                }
                else if (MaxX > obj.MinX && MaxX - obj.MinX <= max)
                {
                    this.MinX -= MaxX - obj.MinX;
                }
                else if (MinX < obj.MaxX && MinX - obj.MaxX >= -max)
                {
                    this.MinX -= MinX - obj.MaxX;
                }
            }
        }


        #endregion

        public virtual void Update(float dt)
        {
            
        }

        /// <summary>
        /// 自分が持っている座標に自分が持っている大きさの矩形を描画する。
        /// </summary>
        /// <param name="g">このグラフィックスコンテクストにオブジェクトを描画する。</param>
        public virtual void Draw()
        {
            if(_isAlive)
            {
                GraphicsContext.FillRectangle(Brushes.Red, MinX, MinY, Width, Height);
            }
        }

    }
}
