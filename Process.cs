using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    delegate void ExecuteWith(EditableObject obj, float dt);

    class Process
    {
        public float MilliSeconds { get; private set; }
        public ExecuteWith ExecuteWith;

        public Process(ExecuteWith executeWith, float seconds)
        {
            this.ExecuteWith = executeWith;
            this.MilliSeconds = seconds * 1000;
        }

    }
}
