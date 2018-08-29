// Common and project usings
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

// Custom usings
using GridEngine.Areas;
using GridEngine.Deligates;
using GridEngine.Entities;
using GridEngine.Enums;
//using GridEngine.Structures;
using GridEngine.Menus;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Engine
{
    public interface ISave
    {
    //- Properties
        string Name { get; }
        
        Dictionary<string, IArea> ModifiedAreas { get; }

        IPlayer ModifiedPlayer { get; }

        Tuple<string, string> StartLocation { get; }
    }
}