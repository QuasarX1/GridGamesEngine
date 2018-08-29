// Common and project usings
using System;
using System.Collections.Generic;
using System.Xml;

// Addidional dependancy usings

// Custom usings
using GridEngine.Engine;
using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Menus
{
    public interface IPauseMenu: IRootMenu
    {
        bool SaveGame(XmlNode saveData, string saveName);
    }
}