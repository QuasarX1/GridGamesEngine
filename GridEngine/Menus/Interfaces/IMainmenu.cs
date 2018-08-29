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
    public interface IMainMenu: IRootMenu// Must contain new game and load save...
    {
        ISave LoadSave(XmlNode saveData, int saveNumber, IGameHost host);

        bool NewGame(XmlNode saveData, string name);
    }
}