using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public delegate void ExecuteWith(IEditable obj, float dt);

    public class Process
    {
        public float MilliSeconds { get; }
        public ExecuteWith ExecuteWith { get; }

        public Process(ExecuteWith executeWith, float seconds)
        {
            this.ExecuteWith = executeWith;
            this.MilliSeconds = seconds * 1000;
        }

    }
}
