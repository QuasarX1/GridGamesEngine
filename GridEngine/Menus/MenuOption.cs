// Common and project usings
using System;
using System.Collections.Generic;
using System.Xml;

// Addidional dependancy usings

// Custom usings
using GridEngine.Deligates;
using GridEngine.Engine;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Menus
{
    public class MenuOption: IMenuOption
    {
    //- Fields and Properties
        public IMenu Parent { get; protected set; }

        public string StaticDataKey { get; protected set; }

        public string Text { get; protected set; }

        public string Image { get; protected set; }

        public string[] Values { get; protected set; }

        public int CurrentIndex { get; protected set; }

        public int SavedCurrentIndex { get; protected set; }



    //- Constructors
        public MenuOption(XmlNode menuNode, IMenu parent)
        {
            // Load the parent
            Parent = parent;

            Text = menuNode.Attributes["text"].Value;

            if (menuNode.Attributes["image"] == null)
            {
                Image = menuNode.Attributes["image"].Value;
            }
            else
            {
                Image = null;
            }

            List<string> values = new List<string>();

            // Load the values
            foreach (XmlNode option in menuNode.ChildNodes)
            {
                values.Add(option.Attributes["data"].Value);
            }

            Values = values.ToArray();

            // Load the current value index
            SavedCurrentIndex = Convert.ToInt16(menuNode.Attributes["current_index"].Value);
            CurrentIndex = SavedCurrentIndex;

            // Load the static data key
            StaticDataKey = menuNode.Attributes["data_key"].Value;

            // Add the current value to the static data store
            Engine.Engine.StaticDataStore[StaticDataKey] = new Structures.StaticData(Values[SavedCurrentIndex]);
        }

    //- Methods
        public bool Save()
        {
            SavedCurrentIndex = CurrentIndex;

            return (bool)OnRaiseUpdateValue();
        }

        public bool Reset()
        {
            if (Values.Length == 0)
            {
                return false;
            }

            CurrentIndex = SavedCurrentIndex;

            return true;
        }

        public bool Next()
        {
            if (Values.Length < 1)
            {
                return false;
            }

            if (++CurrentIndex >= Values.Length)
            {
                CurrentIndex = 0;
            }

            return true;
        }

        public bool Previous()
        {
            if (Values.Length < 1)
            {
                return false;
            }

            if (--CurrentIndex <= 0 )
            {
                CurrentIndex = Values.Length - 1;
            }

            return true;
        }



    //- Events
        public event Func<string, string, bool> UpdateValue;

        protected bool? OnRaiseUpdateValue()
        {
            return UpdateValue?.Invoke(Values[SavedCurrentIndex], StaticDataKey);
        }
    }
}