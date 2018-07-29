using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Entities;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Engine
{
    public sealed class Engine
    {
        public Dictionary<string, IArea> Areas { get; private set; }

        public IPlayer Player { get; private set; }

        public Dictionary<ConsoleKey, Responce> Actions { get; private set; }

        public List<ConsoleKey> ReservedKeys { get; private set; }

        private IArea ActiveArea;

        public string Name { get; private set; }

        private string PreviousName;

        private int[] ConsoleWindowSettings;

        
        public Engine(string name, ref Dictionary<string, IArea> areas)
        {
            PreviousName = Console.Title;
            Console.Title = name;
            Areas = areas;
            Actions = new Dictionary<ConsoleKey, Responce>();
            ReservedKeys = new List<ConsoleKey> { ConsoleKey.Escape };
            Actions[ConsoleKey.Escape] = new Responce(stop => null);
        }

        public Engine(XmlDocument gameData, Type methodsClass, Type entities)
        {
            PreviousName = Console.Title;
            Console.Title = gameData.DocumentElement.Attributes["name"].Value;
            ConsoleWindowSettings = new int[4] { Console.WindowWidth, Console.WindowHeight, Console.WindowTop, Console.WindowLeft };

            // Create areas
            Areas = new Dictionary<string, IArea>();

            foreach (XmlNode node in gameData.DocumentElement.ChildNodes)
            {
                if (node.Name == "area")
                {
                    Areas[node.Attributes["name"].Value] = (IArea)Activator.CreateInstance(Type.GetType(node.Attributes["type"].Value), node, entities, methodsClass);
                }
            }

            //Add player and actions
            Actions = new Dictionary<ConsoleKey, Responce>();
            ReservedKeys = new List<ConsoleKey> { ConsoleKey.Escape };
            Actions[ConsoleKey.Escape] = new Responce(stop => null);
            
            
            foreach (XmlNode node in gameData.DocumentElement.ChildNodes)
            {
                if (node.Name == "player")
                {
                    foreach (XmlNode playerNode in node.ChildNodes)
                    {
                        if (playerNode.Name == "actions")
                        {
                            foreach (XmlNode actionNode in playerNode.ChildNodes)
                            {
                                if (actionNode.Name == "action")
                                {
                                    KeyAction((ConsoleKey)Enum.Parse(typeof(ConsoleKey), actionNode.Attributes["key"].Value), (Responce)Delegate.CreateDelegate(typeof(Responce), methodsClass, actionNode.Attributes["method"].Value));
                                }
                            }

                            break;
                        }
                    }
                    
                    AddPlayer(node, entities.GetNestedType(node.Attributes["type"].Value), entities, methodsClass);

                    break;
                }
            }
        }

        public void AddPlayer(string name, DisplayChar? indicator = null)
        {
            Player = new PlayerEntity(name, Actions, indicator);

            Player.StopEngine += End;
        }

        public void AddPlayer(XmlNode playerXml, Type playerClass, Type entities, Type methodsClass)
        {
            Console.WriteLine(methodsClass);
            Player = (IPlayer)Activator.CreateInstance(playerClass, playerXml, Actions, entities, methodsClass);

            Player.StopEngine += End;
        }

        public void AddPlayer(IPlayer player)
        {
            Player = new PlayerEntity(player, Actions);

            Player.StopEngine += End;
        }

        public void Start()
        {
            if (Player == null)
            {
                throw new InvalidOperationException("No player has yet been defined.");
            }

            Console.CursorVisible = false;
            Console.Clear();

            ActiveArea = (IArea)Areas["Start"].Clone();// Error 1 here "clone". issue with collisions in general even border.

            ActiveArea.ShowArea((IPlayer)Player.Clone(), Actions);
        }

        public void ChangeArea(string areaName, string entryPoint = "deafult")
        {//Error 2 here
            ActiveArea.HideArea();

            ActiveArea = (IArea)Areas[areaName].Clone();

            ActiveArea.ShowArea((IPlayer)Player.Clone(), Actions, entryPoint);
        }

        public void End(object sender, StopEngineEventArgs e)
        {
            // Reset console window
            Console.Clear();
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.SetWindowSize(ConsoleWindowSettings[0], ConsoleWindowSettings[1]);
            Console.WindowTop = ConsoleWindowSettings[2];
            Console.WindowLeft = ConsoleWindowSettings[3];
            Console.Title = PreviousName;

            OnRaiseEngineStop(new EngineStopEventArgs());
        }

        /// <summary>
        /// Assigns a responce to a key.
        /// This will overwrite any currently assigned action.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="responce">A Responce deligate for the responce method.</param>
        public void KeyAction(ConsoleKey key, Responce responce)
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
        public bool AddKeyAction(ConsoleKey key, Responce responce)
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

        public void RemoveKeyAction(ConsoleKey key)
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

        public bool RemoveKeyActionOrFail(ConsoleKey key)
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


        public event EngineStopEventHandler EngineStop;

        public void OnRaiseEngineStop(EngineStopEventArgs e)
        {
            EngineStop?.Invoke(this, e);
        }
    }

    public delegate void EngineStopEventHandler(object sender, EngineStopEventArgs e);

    public class EngineStopEventArgs : EventArgs
    {
        public EngineStopEventArgs()
        {
        }
    }
}