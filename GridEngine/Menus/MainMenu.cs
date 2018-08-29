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
    public class MainMenu: RootMenu, IMainMenu
    {
    //- Fields and Properties
        public object Field;

        public object Property { get; protected set; }



    //- Constructors
        public MainMenu(XmlNode menuNode) : base(menuNode)
        {
            
        }

    //- Methods
        public ISave LoadSave(XmlNode saveData, int saveNumber, IGameHost host)
        {
            return new Save(saveData.ChildNodes[saveNumber - 1], host);
        }

        public bool NewGame(XmlNode saveData, string name)
        {
            if (saveData.ChildNodes.Count > Convert.ToInt16(saveData.Attributes["max_saves"].Value))
            {
                return false;
            }
            else
            {
                //TODO: Edit .gme with new blank save

                return true;
            }
        }
    }
}