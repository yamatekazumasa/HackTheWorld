using System.Collections.Generic;
using System.Linq;

namespace HackTheWorld
{
    /// <summary>
    /// ゲーム内の状態を保存するクラス。
    /// </summary>
    abstract class Scene
    {
        /// <summary>
        /// シーンの履歴。
        /// </summary>
        private static Stack<Scene> _sceneStack = new Stack<Scene>();

        /// <summary>
        /// 現在のシーン。
        /// </summary>
        public static Scene Current
        {
            set
            {
                Scene s = value;
                foreach (Scene scene in _sceneStack) scene.Cleanup();
                s?.Startup();
                _sceneStack = new Stack<Scene>();
                _sceneStack.Push(s);
            }
            get
            {
                return _sceneStack.First();
            }
        }

        /// <summary>
        /// 新しいシーンをスタックにプッシュする。
        /// </summary>
        /// <param name="s"></param>
        public static void Push(Scene s)
        {
            s.Startup();
            _sceneStack.Push(s);
        }

        /// <summary>
        /// 最新のシーンをスタックから取り出す。
        /// </summary>
        public static void Pop()
        {
            var s = _sceneStack.Pop();
            s.Cleanup();
        }

        /// <summary>
        /// 毎フレームの時間を受け取って、シーンを更新する。
        /// </summary>
        public abstract void Update(float dt);
        /// <summary>
        /// シーンをクリアする。
        /// オブジェクトの Dispose 等を行う。
        /// </summary>
        public abstract void Cleanup();
        /// <summary>
        /// シーンを初期化する。
        /// </summary>
        public abstract void Startup();

    }
}
