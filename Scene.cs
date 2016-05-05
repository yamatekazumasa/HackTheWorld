using System.Collections.Generic;
using System.Linq;

namespace HackTheWorld
{
    abstract class Scene
    {
        private static Stack<Scene> _sceneStack = new Stack<Scene>();

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

        public static void Push(Scene s)
        {
            s.Startup();
            _sceneStack.Push(s);
        }

        public static void Pop()
        {
            var s = _sceneStack.Pop();
            s.Cleanup();
        }

        public abstract void Update(float dt);
        public abstract void Cleanup();
        public abstract void Startup();

    }
}
