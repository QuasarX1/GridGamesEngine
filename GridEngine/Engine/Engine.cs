using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

using GridEngine.Areas;
using GridEngine.Deligates;
using GridEngine.Entities;
using GridEngine.Enums;
using GridEngine.Structures;
using GridEngine.Menus;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Engine
{
    public sealed class Engine
    {
    //- Fields and Properties
        public IGameHost Host { get; private set; }
        public string Name { get; private set; }

        private XmlDocument GameData;

        public Dictionary<string, IArea> Areas { get; private set; }

        public IArea ActiveArea { get; private set; }

        public Dictionary<string, IRootMenu> Menus { get; private set; }

        public IMainMenu MainMenu { get; private set; }

        public IRootMenu ActiveMenuRoot { get; private set; }

        public IMenu ActiveMenu { get; private set; }

        public IPlayer Player { get; private set; }

        public List<Keys> ReservedKeys { get; private set; }

        public Dictionary<Keys, Tuple<Responce, string[]>> Actions { get; private set; }

        public Dictionary<string, string> Images { get; private set; }

        public static Dictionary<string, StaticData> StaticDataStore;//TODO: add into xml and read in constructors of objects



    //- Constructors
        // use host's other events to pause/resume ect...
        public Engine(XmlDocument gameData, IGameHost host)
        {
            Host = host;

        //- Validate the XML document
            GameData = gameData;
            GameData.Schemas.Add(null, "https://raw.githubusercontent.com/QuasarX1/GridGamesEngine/master/GridEngine/GridGamesData.xsd");

            //void ValidationEventHandler(object sender, ValidationEventArgs e)
            //{
            //    throw new XmlSchemaException("The format of the game data was invalid.");
            //}
            
            GameData.Validate(new ValidationEventHandler((object sender, ValidationEventArgs e) => throw new XmlSchemaException("The format of the game data was invalid. The schema the XML must conform to can be found at \"https://raw.githubusercontent.com/QuasarX1/GridGamesEngine/master/GridEngine/GridGamesData.xsd\"")));

            Name = GameData.DocumentElement.Attributes["name"].Value;

        //- Create areas
            XmlNode gameSubNode = GameData.DocumentElement.FirstChild;// selects the "areas" node

            Areas = new Dictionary<string, IArea>();

            foreach (XmlNode node in gameSubNode)
            {
                Areas[node.Attributes["name"].Value] = (IArea)Activator.CreateInstance(Type.GetType(node.Attributes["type"].Value), node, host);
            }
            
        //- Add player and actions
            Actions = new Dictionary<Keys, Tuple<Responce, string[]>>();
            ReservedKeys = new List<Keys> { Keys.Escape };
            // TODO: Make pause menu
            Actions[Keys.Escape] = new Tuple<Responce, string[]>(new Responce((IPlayer player, string[] args) => null), new string[0]);

            gameSubNode = gameSubNode.NextSibling; ;// selects the "player" node

            foreach (XmlNode playerSubNode in gameSubNode.ChildNodes)
            {
                foreach (XmlNode actionNode in playerSubNode.LastChild)// The actions node is the final child of the player
                {
                    KeyAction((Keys)Enum.Parse(typeof(Keys), actionNode.Attributes["key"].Value), (Responce)Delegate.CreateDelegate(typeof(Responce), typeof(Methods).GetMethod(actionNode.Attributes["method"].Value)), actionNode.ChildNodes);
                }
            }

            AddPlayer(gameSubNode, Type.GetType("GridEngine.Entities." + gameSubNode.Attributes["type"].Value));

        //- Add images
            gameSubNode = gameSubNode.NextSibling; ;// selects the "images" node

            Images = new Dictionary<string, string>();

            foreach (XmlNode imageNode in gameSubNode)
            {
                Images[imageNode.Attributes["name"].Value] = imageNode.Attributes["filename"].Value;
            }
        }


    //- Addition methods
        public void AddPlayer(XmlNode playerXml, Type playerClass)
        {
            Player = (IPlayer)Activator.CreateInstance(playerClass, playerXml, Actions);

            Player.StopEngine += End;
        }
        
        public void AddPlayer(IPlayer player)
        {
            Player = new PlayerEntity(player, Actions);

            Player.StopEngine += End;
        }

        /// <summary>
        /// Assigns a responce to a key.
        /// This will overwrite any currently assigned action.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="responce">A Responce deligate for the responce method.</param>
        public void KeyAction(Keys key, Responce responce, XmlNodeList stringNodes)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new InvalidOperationException("The key " + key.ToString() + " is listed as a reserved key. This can't be assigned a custom action.");
            }

            List<string> args = new List<string>();

            foreach (XmlNode stringNode in stringNodes)
            {
                args.Add(stringNode.Attributes["data"].Value);
            }

            Actions[key] = new Tuple<Responce, string[]>(responce, args.ToArray());
        }

        /// <summary>
        /// Assigns a responce to a key if that key is currently has no associated action.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="responce">A Responce deligate for the responce method.</param>
        /// <returns>Boolean to indicate success or failure.</returns>
        public bool AddKeyAction(Keys key, Responce responce, XmlNodeList stringNodes)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new InvalidOperationException("The key " + key.ToString() + " is listed as a reserved key. This can't be assigned a custom action.");
            }

            if (!Actions.ContainsKey(key))
            {
                List<string> args = new List<string>();

                foreach (XmlNode stringNode in stringNodes)
                {
                    args.Add(stringNode.Attributes["data"].Value);
                }

                Actions[key] = new Tuple<Responce, string[]>(responce, args.ToArray());

                return true;
            }
            else
            {
                return false;
            }
        }


    //- Removal methods
        public void RemoveKeyAction(Keys key)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new InvalidOperationException("The key " + key.ToString() + " is listed as a reserved key. This key's action can't be removed.");
            }

            if (Actions.ContainsKey(key))
            {
                Actions.Remove(key);
            }
        }

        public bool RemoveKeyActionOrFail(Keys key)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new InvalidOperationException("The key " + key.ToString() + " is listed as a reserved key. This key's action can't be removed.");
            }

            if (Actions.ContainsKey(key))
            {
                Actions.Remove(key);

                return true;
            }
            else
            {
                return false;
            }
        }


        //- Operation methods
        public void Start(string startAreaName = "Start")
        {
            if (Player == null)
            {
                throw new InvalidOperationException("No player has yet been defined.");
            }
            
            ActiveArea = (IArea)Areas["Start"].Clone();

            ActiveArea.ShowArea((IPlayer)Player.Clone());//, Actions);
        }

        public void ChangeArea(string areaName, string entryPoint = "deafult")
        {
            ActiveArea.HideArea();

            ActiveArea = (IArea)Areas[areaName].Clone();

            ActiveArea.ShowArea((IPlayer)Player.Clone(), entryPoint);//, Actions, entryPoint);
        }

        public void OpenMenu(string menuName)
        {
            ActiveArea.Pause();// Pause area

            ActiveMenuRoot = Menus[menuName];// Open menu

            NavigateMenu(ActiveMenuRoot);
        }

        public void NavigateMenu(IMenu nextMenu)// Event handler
        {

        }

        public void CloseMenu()
        {
            ActiveMenu = null;

            ActiveMenuRoot = null;

            ActiveArea.Resume();// Resume area
        }

        public void End(object sender, StopEngineEventArgs e)
        {
            OnRaiseEngineStopped(new EngineStoppedEventArgs());
        }

        

    //- Events
        public event EngineStoppedEventHandler EngineStop;

        public void OnRaiseEngineStopped(EngineStoppedEventArgs e)
        {
            EngineStop?.Invoke(this, e);
        }
    }

    public delegate void EngineStoppedEventHandler(object sender, EngineStoppedEventArgs e);

    public class EngineStoppedEventArgs : EventArgs
    {
        public EngineStoppedEventArgs()
        {
        }
    }
}
