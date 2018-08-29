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
    public class RootMenu: Menu, IRootMenu
    {
    //- Constructors
        public RootMenu(XmlNode menuNode) : base(menuNode) { }


    //- Methods
        public void Close()
        {
            OnRaiseCloseMenu();
        }

        public void Open()
        {
            OnRaiseOpenMenu();
        }



    //- Events
        public event Action CloseMenu;

        protected void OnRaiseCloseMenu()
        {
            CloseMenu?.Invoke();
        }


        public event Action<IRootMenu> OpenMenu;

        protected void OnRaiseOpenMenu()
        {
            OpenMenu?.Invoke(this);
        }
    }
}