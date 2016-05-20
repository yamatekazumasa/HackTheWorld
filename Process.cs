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
        public float ElapsedTime { get; set; }

        public Process(ExecuteWith executeWith)
        {
            ExecuteWith = executeWith;
            MilliSeconds = 0;
            ElapsedTime = 0;
        }

        public Process(ExecuteWith executeWith, float seconds)
        {
            ExecuteWith = executeWith;
            MilliSeconds = seconds * 1000;
            ElapsedTime = 0;
        }

    }
}
