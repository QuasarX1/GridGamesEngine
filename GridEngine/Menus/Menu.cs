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
    public abstract class Menu: IMenu
    {
    //- Fields and Properties
        public string Title { get; protected set; }

        public Dictionary<string, Tuple<string, IMenuItem, string>> Options { get; protected set; }



    //- Constructors
        public Menu(XmlNode menuNode)
        {
            Title = menuNode.Attributes["title"].Value;

            Options = new Dictionary<string, Tuple<string, IMenuItem, string>>();

            foreach (XmlNode menuItem in menuNode.ChildNodes)
            {
                if (menuItem.Name == "menu")
                {
                    SubMenu newSubMenu = new SubMenu(menuItem, this);
                    Options[menuItem.Attributes["title"].Value] = new Tuple<string, IMenuItem, string>(newSubMenu.Text, newSubMenu, newSubMenu.Image);
                }
                else if (menuItem.Name == "menu_option")
                {
                    MenuOption newMenuOption = new MenuOption(menuItem, this);
                    Options[menuItem.Attributes["name"].Value] = new Tuple<string, IMenuItem, string>(newMenuOption.Text, newMenuOption, newMenuOption.Image);
                }
                else if (menuItem.Name == "menu_trigger")
                {
                    MenuTrigger newMenuOption = new MenuTrigger(menuItem, this);
                    Options[menuItem.Attributes["name"].Value] = new Tuple<string, IMenuItem, string>(newMenuOption.Text, newMenuOption, newMenuOption.Image);
                }
            }
        }

    //- Methods
        public void Select(string itemName)
        {
            if (Options[itemName].Item2 is ISubMenu)
            {
                OnRaiseMenuNavigation((ISubMenu)Options[itemName].Item2);
            }
            else if (Options[itemName].Item2 is IMenuOption)
            {
                ((IMenuOption)Options[itemName].Item2).Next();
            }
        }



    //- Events
        public event Action<IMenu> MenuNavigation;

        protected void OnRaiseMenuNavigation(IMenu e)
        {
            MenuNavigation?.Invoke(e);
        }
    }
}