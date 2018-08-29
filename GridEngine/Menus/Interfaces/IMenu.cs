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
    public interface IMenu
    {
    //- Properties
        string Title { get; }

        Dictionary<string, Tuple<string, IMenuItem, string>> Options { get; } // Item name | { Text | Item | Image name (optional) }



        //- Methods
        void Select(string itemName);



    //- Events
        event Action<IMenu> MenuNavigation;
    }
}