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
    public interface IMenuItem
    {
    //- Properties
        IMenu Parent { get; }

        string Name { get; }

        string Text { get; }

        string Image { get; }
    }
}