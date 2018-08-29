using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Menus;
using static GridEngine.Enums.InputKeys;

namespace GridEngine
{
    public interface IGameHost
    {
        //- Operation methods
        void OnMenuLoad(IRootMenu newMenu);

        void OnMenuNavigation(IMenu newMenu);

        void OnMenuClose();
        
        
        
    //- Events
        event Action<Keys> KeyPress;

        event Action Suspend;

        event Action Resume;

        event Action Terminate;
    }
}
