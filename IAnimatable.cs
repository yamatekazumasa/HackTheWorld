using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackTheWorld
{
    public delegate void OnComplete(IAnimatable anim);

    public interface IAnimatable
    {
        float ElapsedTime { get; set; }
        AnimationServer AnimationServer { get; set; }
        bool IsPlaying { get; set; }
        OnComplete OnComplete { get; set; }
        void Advance(float dt);

        void Start(AnimationServer server);
        void Stop();
    }

    public partial class Extensions
    {
        public static void Start(this IAnimatable self, AnimationServer server)
        {
            self.AnimationServer = server;
            self.ElapsedTime = 0.0f;
            self.IsPlaying = true;
        }

        public static void Stop(this IAnimatable self)
        {
            self.OnComplete?.Invoke(self);
            self.IsPlaying = false;
        }

        public static void Rewind(this IAnimatable self)
        {
            self.ElapsedTime = 0.0f;
        }

        public static void Update(this IAnimatable self, float dt)
        {
            self.ElapsedTime += dt;
            self.Advance(dt);
        }

    }
}
