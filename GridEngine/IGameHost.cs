using System;
using System.Collections.Generic;
using System.Text;

using static GridEngine.Enums.InputKeys;

namespace GridEngine
{
    public interface IGameHost
    {
        event Action<Keys> KeyPress;

        event Action Suspend;

        event Action Resume;

        event Action Terminate;
    }
}
