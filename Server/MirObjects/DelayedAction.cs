using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.MirObjects
{
    public enum DelayedType
    {
        Magic,
        Damage,
        Spawn,
        Recall,
        MapMovement
    }

    public class DelayedAction
    {
        public DelayedType Type;
        public long Time;
        public object[] Params;

        public DelayedAction(DelayedType type, long time, params object[] p)
        {
            Type = type;
            Time = time;
            Params = p;
        }
    }
}
