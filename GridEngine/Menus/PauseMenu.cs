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
    public class PauseMenu: RootMenu, IPauseMenu
    {
    //- Fields and Properties
        public object Field;

        public object Property { get; protected set; }



    //- Constructors
        public PauseMenu(XmlNode menuNode) : base(menuNode)
        {
            
        }

    //- Methods
        public bool SaveGame(XmlNode saveData, string saveName)
        {
            XmlNode correctSaveData = null;

            foreach (XmlNode save in saveData)
            {
                if (save.Attributes["name"].Value == saveName)
                {
                    correctSaveData = save;


                    break;
                }
            }

            if (correctSaveData == null)
            {
                return false;
            }
            else
            {
                //TODO: update the save

                return true;
            }
        }
    }
}