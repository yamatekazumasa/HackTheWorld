using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    delegate void ExecuteWith(GameObject obj);

    class Process
    {
        public int Frame { get; private set; }
        public ExecuteWith ExecuteWith;

        public Process(ExecuteWith executeWith, int frame)
        {
            this.ExecuteWith = executeWith;
            this.Frame = frame;
        }
    }
}
