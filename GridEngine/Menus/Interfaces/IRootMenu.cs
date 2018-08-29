// Common and project usings
using System;
using System.Collections.Generic;

// Addidional dependancy usings

// Custom usings
using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Menus
{
    public interface IRootMenu: IMenu
    {
    //- Methods
        void Close();

        void Open();



    //- Events
        event Action CloseMenu;

        event Action<IRootMenu> OpenMenu;
    }
}