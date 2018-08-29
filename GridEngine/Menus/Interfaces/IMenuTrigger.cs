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
    public interface IMenuTrigger: IMenuItem
    {
    //- Properties
        bool Active { get; }

        string TriggerValue { get; }



        //- Methods
        bool Pressed();



    //- Events
        event Func<string, bool> Trigger;
    }
}