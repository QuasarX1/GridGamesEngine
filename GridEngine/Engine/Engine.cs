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
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Engine
{
    public sealed class Engine
    {
        public string Name { get; private set; }

        private XmlDocument GameData;

        public Dictionary<string, IArea> Areas { get; private set; }

        public IArea ActiveArea { get; private set; }

        public IPlayer Player { get; private set; }

        public List<Keys> ReservedKeys { get; private set; }

        public Dictionary<Keys, Responce> Actions { get; private set; }

        public Dictionary<string, string> Images { get; private set; }


        public Engine(XmlDocument gameData)
        {
            // Validate the XML document
            GameData = gameData;
            GameData.Schemas.Add(null, "https://raw.githubusercontent.com/QuasarX1/GridGamesEngine/master/GridEngine/GridGamesData.xsd");

            //void ValidationEventHandler(object sender, ValidationEventArgs e)
            //{
            //    throw new XmlSchemaException("The format of the game data was invalid.");
            //}
            
            GameData.Validate(new ValidationEventHandler((object sender, ValidationEventArgs e) => throw new XmlSchemaException("The format of the game data was invalid. The schema the XML must conform to can be found at \"https://raw.githubusercontent.com/QuasarX1/GridGamesEngine/master/GridEngine/GridGamesData.xsd\"")));

            Name = GameData.DocumentElement.Attributes["name"].Value;

            // Create areas
            XmlNode gameSubNode = GameData.DocumentElement.FirstChild;// selects the "areas" node

            Areas = new Dictionary<string, IArea>();

            foreach (XmlNode node in gameSubNode)
            {
                Areas[node.Attributes["name"].Value] = (IArea)Activator.CreateInstance(Type.GetType(node.Attributes["type"].Value), node);
            }
            
            // Add player and actions
            Actions = new Dictionary<Keys, Responce>();
            ReservedKeys = new List<Keys> { Keys.Escape };
            // TODO: Make pause menu
            Actions[Keys.Escape] = new Responce(stop => null);

            gameSubNode = gameSubNode.NextSibling; ;// selects the "player" node

            foreach (XmlNode playerSubNode in gameSubNode)
            {
                foreach (XmlNode actionNode in playerSubNode.LastChild)// The actions node is the final child of the player
                {
                    KeyAction((ConsoleKey)Enum.Parse(typeof(ConsoleKey), actionNode.Attributes["key"].Value), (Responce)Delegate.CreateDelegate(typeof(Responce), methodsClass, actionNode.Attributes["method"].Value));
                }
            }

            AddPlayer(gameSubNode, Type.GetType(gameSubNode.Attributes["type"].Value));

            // Add images
            gameSubNode = gameSubNode.NextSibling; ;// selects the "images" node

            Images = new Dictionary<string, string>();

            foreach (XmlNode imageNode in gameSubNode)
            {
                Images[imageNode.Attributes["name"].Value] = imageNode.Attributes["filename"].Value;
            }
        }

        public void AddPlayer(XmlNode playerXml, Type playerClass)
        {
            Player = (IPlayer)Activator.CreateInstance(playerClass, playerXml, Actions);

            Player.StopEngine += End;
        }

        //TODO: Obsolite???
        public void AddPlayer(IPlayer player)
        {
            Player = new PlayerEntity(player, Actions);

            Player.StopEngine += End;
        }

        public void Start(string startAreaName = "Start")
        {
            if (Player == null)
            {
                throw new InvalidOperationException("No player has yet been defined.");
            }
            
            ActiveArea = (IArea)Areas["Start"].Clone();

            ActiveArea.ShowArea((IPlayer)Player.Clone(), Actions);
        }

        public void ChangeArea(string areaName, string entryPoint = "deafult")
        {
            ActiveArea.HideArea();

            ActiveArea = (IArea)Areas[areaName].Clone();

            ActiveArea.ShowArea((IPlayer)Player.Clone(), Actions, entryPoint);
        }

        public void OpenMenu()
        {
            // Pause area
            // Open menu
        }

        public void CloseMenu()
        {
            // change to event handeler
            // Resume area
        }

        public void End(object sender, StopEngineEventArgs e)
        {
            OnRaiseEngineStopped(new EngineStoppedEventArgs());
        }

        /// <summary>
        /// Assigns a responce to a key.
        /// This will overwrite any currently assigned action.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="responce">A Responce deligate for the responce method.</param>
        public void KeyAction(Keys key, Responce responce)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new InvalidOperationException("The key " + key.ToString() + " is listed as a reserved key. This can't be assigned a custom action.");
            }

            Actions[key] = responce;
        }

        /// <summary>
        /// Assigns a responce to a key if that key is currently has no associated action.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="responce">A Responce deligate for the responce method.</param>
        /// <returns>Boolean to indicate success or failure.</returns>
        public bool AddKeyAction(Keys key, Responce responce)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new InvalidOperationException("The key " + key.ToString() + " is listed as a reserved key. This can't be assigned a custom action.");
            }

            if (!Actions.ContainsKey(key))
            {
                Actions[key] = responce;
                return true;
            }
            else
            {
                return false;
            }
        }

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
