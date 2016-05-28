using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    public class AnimationServer
    {
        public static AnimationServer Server { get; } = new AnimationServer();

        private List<IAnimatable> _animations;

        public void Advance(float dt)
        {
            _animations.ForEach(a => a.Advance(dt));
        }

        public void Add(IAnimatable a)
        {
            _animations.Add(a);
            a.Start(this);
        }

        public void Remove(IAnimatable anim)
        {
            _animations.Remove(anim);
            anim.Stop();
        }

        public void RemoveAll()
        {
            _animations.ForEach(a => a.Stop());
            _animations = new List<IAnimatable>();
        }

    }
}
