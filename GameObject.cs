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
        public GameObject() { }

        /// <summary>
        ///  位置を指定してオブジェクトを出現させる。
        /// </summary>
        /// <param name="x">初期x座標。</param>
        /// <param name="y">初期y座標。</param>
        public GameObject(int x, int y) : this()
        {
            SetPosition(x, y);
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
            SetVelocity(vx, vy);
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
            SetSize(w, h);
        }

        #endregion


        #region アクセサ

        // まだ追加途中
        public Vector Position 
        {
            get { return _position/Scale; }
            set { _position = value*Scale; }
        }

        public Vector Velocity
        {
            get { return _velocity/Scale*10; }
            set { _velocity = value*Scale/10; }
        }

        /// <summary>
        /// 中央の位置を指定する。
        /// </summary>
        /// <param name="x">x座標。</param>
        /// <param name="y">y座標。</param>
        /// <returns></returns>
        public void SetPosition(int x, int y)
        {
            this._position = new Vector(x, y) * Scale;
        }

        public void SetPosition(Vector v)
        {
            this._position = v * Scale;
        }

        /// <summary>
        /// 速度を指定する。
        /// </summary>
        /// <param name="vx">速度のx方向成分。</param>
        /// <param name="vy">速度のy方向成分。</param>
        /// <returns></returns>
        public void SetVelocity(int vx, int vy)
        {
            this._velocity = new Vector(vx, vy) * Scale / 10;
        }

        public void SetVelocity(Vector v)
        {
            this._velocity = v * Scale / 10;
        }

        /// <summary>
        /// サイズを指定する。
        /// </summary>
        /// <param name="w">幅。</param>
        /// <param name="h">高さ。</param>
        /// <returns></returns>
        public void SetSize(int w, int h)
        {
            this._size = new Vector(w, h) * Scale;
        }

        public void SetSize(Vector v)
        {
            this._size = v * Scale;
        }

        /// <summary>
        /// 中央の座標を取得する。
        /// </summary>
        /// <returns></returns>
        public Vector GetPosition()
        {
            return _position / Scale;
        }

        /// <summary>
        /// 速度を取得する。
        /// </summary>
        /// <returns></returns>
        public Vector GetVelocity()
        {
            return _velocity / Scale * 10;
        }

        /// <summary>
        /// サイズを取得する。
        /// </summary>
        /// <returns></returns>
        public Vector GetSize()
        {
            return _size / Scale;
        }

        /// <summary>
        /// 左上のx座標を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetMinX()
        {
            return (int)((_position.X - _size.X / 2) / Scale);
        }

        /// <summary>
        /// 左上のy座標を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetMinY()
        {
            return (int)((_position.Y - _size.Y / 2) / Scale);
        }

        /// <summary>
        /// 右下のx座標を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetMaxX()
        {
            return (int)((_position.X + _size.X / 2) / Scale);
        }

        /// <summary>
        /// 右下のy座標を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetMaxY()
        {
            return (int)((_position.Y + _size.Y / 2) / Scale);
        }

        /// <summary>
        /// 中央のx座標を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetMidX()
        {
            return (int)(_position.X / Scale);
        }

        /// <summary>
        /// 中央のy座標を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetMidY()
        {
            return (int)(_position.Y / Scale);
        }

        /// <summary>
        /// 横幅を取得する。
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return (int)(_size.X / Scale);
        }

        /// <summary>
        /// 高さを取得する。
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return (int)(_size.Y / Scale);
        }

        /// <summary>
        /// オブジェクトを消す。
        /// </summary>
        public virtual void Die()
        {
            this._isAlive = false;
        }

        /// <summary>
        /// オブジェクトのタイプを返す。
        /// </summary>
        /// <returns></returns>
        public virtual ObjectType GetObjectType()
        {
            return this._objectType;
        }

        #endregion


        /// <summary>
        /// 初期化用。
        /// </summary>
        public void Initialize()
        {
            this._isAlive = true;
            this.SetSize(30, 30);
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
            SetVelocity(vx, vy);
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
            return GetMinX() < obj.GetMaxX() && GetMaxX() > obj.GetMinX() &&
                   GetMinY() < obj.GetMaxY() && GetMaxY() > obj.GetMinY();
        }

        /// <summary>
        /// 包含判定。
        /// 渡されたオブジェクトの矩形領域を包含しているか判定する。
        /// </summary>
        /// <param name="obj">渡されたオブジェクト。</param>
        /// <returns>包含していたらtrue、包含していなかったらfalseを返す。</returns>
        public virtual bool Contains(GameObject obj)
        {
            return GetMinX() < obj.GetMinX() && GetMaxX() > obj.GetMaxX() &&
                   GetMinY() < obj.GetMinY() && GetMaxY() > obj.GetMaxY();
        }

        /// <summary>
        /// 衝突判定。
        /// </summary>
        /// <param name="obj">渡されたオブジェクトと衝突しているか判定する。</param>
        /// <returns>衝突していたらtrue、衝突していなかったらfalseを返す。</returns>
        public virtual bool CollideWith(GameObject obj)
        {
            if (!obj._isAlive) return false;
            if (_objectType == obj._objectType) return false;
            return this.Intersects(obj);
        }

        /// <summary>
        /// オブジェクトがウィンドウの中に納まっているか判定する。
        /// </summary>
        /// <returns>オブジェクトがウィンドウ内にあればture、ウインドウ外にあればfalseを返す。</returns>
        public virtual bool InWindow()
        {
            return GetMinX() > -100 && GetMinX() < ScreenWidth + 100 &&
                   GetMinY() > -100 && GetMinY() < ScreenHeight + 100;
        }

        #endregion

        /// <summary>
        /// 自分が持っている座標に自分が持っている大きさの矩形を描画する。
        /// </summary>
        /// <param name="g">このグラフィックスコンテクストにオブジェクトを描画する。</param>
        public virtual void Draw()
        {
            if(this._isAlive)
            {
                GraphicsContext.FillRectangle(Brushes.Red, GetMinX(), GetMinY(), GetWidth(), GetHeight());
            }
        }

    }
}
