using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class GameObject
    {
        /// <summary>
        /// 位置。
        /// </summary>
        private Vector _position;
        /// <summary>
        /// 速度。
        /// </summary>
        private Vector _velocity;
        /// <summary>
        /// サイズ。衝突判定に用いる。矩形で与えられる。
        /// </summary>
        private Vector _size;
        /// <summary>
        /// 生死フラグ。基本はdelete以外で弄らないように。
        /// </summary>
        private bool _isAlive;
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
            Position = new Vector(x, y);
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
            Velocity = new Vector(vx, vy);
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
            Size = new Vector(w, h);
        }

        #endregion


        #region アクセサ

        public Vector Position
        {
            get { return _position/Scale; }
            set { _position = value*Scale; }
        }

        public Vector Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector Size
        {
            get { return _size/Scale; }
            set { _size = value*Scale; }
        }

        public int MinX
        {
            get { return (int) (_position.X/Scale); }
            set { _position.X = value*Scale; }
        }

        public int MinY
        {
            get { return (int) (_position.Y/Scale); }
            set { _position.Y = value*Scale; }
        }

        public int MaxX
        {
            get { return (int) ((_position.X + _size.X)/Scale); }
            set { _position.X = value*Scale - _size.X; }
        }

        public int MaxY
        {
            get { return (int) ((_position.Y + _size.Y)/Scale); }
            set { _position.Y = value*Scale - _size.Y; }
        }

        public int MidX
        {
            get { return (int) ((_position.X + _size.X/2)/Scale); }
            set { _position.X = value*Scale - _size.X/2; }
        }

        public int MidY
        {
            get { return (int) ((_position.Y + _size.Y/2)/Scale); }
            set { _position.Y = value*Scale - _size.Y/2; }
        }

        public int X
        {
            get { return MinX; }
            set { MinX = value; }
        }

        public int Y
        {
            get { return MinY; }
            set { MinY = value; }
        }

        public int VX
        {
            get { return (int)_velocity.X; }
            set { _velocity.X = value; }
        }

        public int VY
        {
            get { return (int)_velocity.Y; }
            set { _velocity.Y = value; }
        }

        public int Width => (int)(_size.X / Scale);
        public int Height => (int)(_size.Y / Scale);
        public ObjectType ObjectType => _objectType;


        /// <summary>
        /// オブジェクトを消す。
        /// </summary>
        public virtual void Die()
        {
            this._isAlive = false;
        }

        #endregion


        /// <summary>
        /// 初期化用。
        /// </summary>
        public void Initialize()
        {
            this._isAlive = true;
            Size = new Vector(Cell, Cell);
        }
        /// <summary>
        /// 初期化用。
        /// </summary>
        /// <param name="type">オブジェクトタイプ。</param>
        public void Initialize(ObjectType type)
        {
            this._objectType = type;
        }

        #region GameObject専用

        /// <summary>
        /// 設定された速度で1フレーム分動く。
        /// </summary>
        public virtual void Move()
        {
            _position += _velocity;
        }

        /// <summary>
        /// 渡された速度で1フレーム分動く。
        /// </summary>
        public virtual void Move(int vx, int vy)
        {
            Velocity = new Vector(vx, vy);
            Move();
        }
        
        /// <summary>
        /// 角度を指定して、velocityを回転させる。
        /// </summary>
        /// <param name="deg">回転角。degreeで指定する。</param>
        /// <returns></returns>
        public virtual void Rotate(double deg)
        {
            this._velocity = this._velocity.Rotate(deg);
        }

        /// <summary>
        /// 加速度を指定して、velocityに加える。
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public virtual void Accelerate(double a)
        {
            this._velocity = this._velocity.Extend(a);
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
        /// 衝突判定。
        /// </summary>
        /// <param name="obj">渡されたオブジェクトと衝突しているか判定する。</param>
        /// <returns>衝突していたらtrue、衝突していなかったらfalseを返す。</returns>
        public virtual bool CollidesWith(GameObject obj)
        {
            if (!obj._isAlive) return false;
            if (_objectType == obj.ObjectType) return false;
            return this.Intersects(obj);
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
            if (this.Intersects(obj))
            {
                int max = 10;// めり込み許容量。10という値は仮で、要調整。
                if (MaxY > obj.MinY && MaxY - obj.MinY <= max)
                {
                    this._position.Y -= (MaxY - obj.MinY) * Scale;
                }
                else if (MinY < obj.MaxY && MinY - obj.MaxY >= -max)
                {
                    this._position.Y -= (MinY - obj.MaxY) * Scale;
                }
                else if (MaxX > obj.MinX && MaxX - obj.MinX <= max)
                {
                    this._position.X -= (MaxX - obj.MinX) * Scale;
                }
                else if (MinX < obj.MaxX && MinX - obj.MaxX >= -max)
                {
                    this._position.X -= (MinX - obj.MaxX) * Scale;
                }
            }
        }


        #endregion

        public virtual void Update()
        {
            
        }

        /// <summary>
        /// 自分が持っている座標に自分が持っている大きさの矩形を描画する。
        /// </summary>
        /// <param name="g">このグラフィックスコンテクストにオブジェクトを描画する。</param>
        public virtual void Draw()
        {
            if(this._isAlive)
            {
                GraphicsContext.FillRectangle(Brushes.Red, MinX, MinY, Width, Height);
            }
        }

    }
}
