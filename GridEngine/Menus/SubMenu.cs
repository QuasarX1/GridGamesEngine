// Common and project usings
using System;
using System.Collections.Generic;
using System.Xml;

// Addidional dependancy usings

// Custom usings
using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Menus
{
    public class SubMenu: Menu, ISubMenu
    {
    //- Fields and Properties
        public IMenu Parent { get; protected set; }

        public string Text { get; protected set; }

        public string Image { get; protected set; }



        //- Constructors
        public SubMenu(XmlNode menuNode, IMenu parent) : base(menuNode)
        {
            Text = menuNode.Attributes["text"].Value;

            if (menuNode.Attributes["image"] == null)
            {
                Image = menuNode.Attributes["image"].Value;
            }
            else
            {
                Image = null;
            }
        }

    //- Methods
        public void Previous()
        {
            OnRaiseMenuNavigation(Parent);
        }
    }
}