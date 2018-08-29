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
    public class Save: ISave
    {
    //- Fields and Properties
        public string Name { get; protected set; }

        public Dictionary<string, IArea> ModifiedAreas { get; protected set; }

        public IPlayer ModifiedPlayer { get; protected set; }

        public Tuple<string, string> StartLocation { get; protected set; }



    //- Constructors
        public Save(XmlNode saveData, IGameHost host)
        {
            Name = saveData.Attributes["name"].Value;

            ModifiedAreas = new Dictionary<string, IArea>();

            foreach (XmlNode area in saveData.ChildNodes[0].ChildNodes)
            {
                ModifiedAreas[area.Attributes["name"].Value] = (IArea)Activator.CreateInstance(Type.GetType(area.Attributes["type"].Value), area, host);
            }

            ModifiedPlayer = new PlayerEntity(saveData.ChildNodes[1], new Dictionary<Keys, Tuple<Responce, string[]>>());

            StartLocation = new Tuple<string, string>(saveData.ChildNodes[2].Attributes["area"].Value, saveData.ChildNodes[2].Attributes["entry_point"].Value);
        }
    }
}