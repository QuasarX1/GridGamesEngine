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
    public interface IMenuOption: IMenuItem
    {
    //- Properties
        string StaticDataKey { get; }

        string[] Values { get; }

        int CurrentIndex { get; }

        int SavedCurrentIndex { get; }



    //- Methods
        bool Save();

        bool Reset();

        bool Next();

        bool Previous();



    //- Events
        event Func<string, string, bool> UpdateValue;
    }
}