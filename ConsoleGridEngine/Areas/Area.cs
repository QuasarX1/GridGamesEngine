using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Entities;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Areas
{
    public class Area: IArea
    {
        public DisplayChar?[,] Grid { get; protected set; }

        public bool Border { get; protected set; }

        public Dictionary<string, int[]> EntryPoints { get; protected set; }

        public DisplayChar EmptyChar { get; protected set; }

        public bool Focus { get; protected set; }

        public Dictionary<string, IEntity> Entities { get; protected set; }

        public System.Threading.Thread ControllPlayerThread { get; protected set; }

        public List<System.Threading.Thread> ControllNPCThreads { get; protected set; }

        public string PlayerName { get; protected set; }


        public Area(int width, int height, bool border = false, DisplayChar? emptyChar = null)
        {
            if (emptyChar == null)
            {
                emptyChar = new DisplayChar { Char = ' ', Colour = ConsoleColor.White };
            }
            
            Border = border;

            if (Border == true)
            {
                width += 2;
                height += 2;
            }

            Grid = new DisplayChar?[width, height];

            EmptyChar = (DisplayChar)emptyChar;

            Focus = false;

            Entities = new Dictionary<string, IEntity>();

            EntryPoints = new Dictionary<string, int[]>();

            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    if (Border == true && (i == 0 || i == Grid.GetLength(0) - 1 || j == 0 || j == Grid.GetLength(1) - 1))
                    {
                        Grid[i, j] = new DisplayChar { Char = 'B', Colour = ConsoleColor.Green };
                    }
                    else
                    {
                        Grid[i, j] = null;
                    }
                }
            }
        }

        public Area(IArea area)
        {
            // Duplicate the border boolean
            Border = area.Border;
            
            Grid = new DisplayChar?[area.Grid.GetLength(0), area.Grid.GetLength(1)];

            EmptyChar = area.EmptyChar;

            Focus = area.Focus;

            Entities = new Dictionary<string, IEntity>();

            foreach (KeyValuePair<string, IEntity> entity in area.Entities)
            {
                //Entities[entity.Key] = (IEntity)entity.Value.Clone();
                AddEntity((IEntity)entity.Value.Clone(), entity.Value.Location);
            }

            EntryPoints = new Dictionary<string, int[]>();

            foreach (KeyValuePair<string, int[]> entry in area.EntryPoints)
            {
                EntryPoints[entry.Key] = entry.Value;
            }

            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = area.Grid[i, j];
                }
            }
        }

        public Area(XmlNode areaXml, Type entities, Type methodsClass)
        {
            Border = Convert.ToBoolean(areaXml.Attributes["border"].Value);

            int width = Convert.ToInt32(areaXml.Attributes["width"].Value);
            int height = Convert.ToInt32(areaXml.Attributes["height"].Value);

            if (Border == true)
            {
                width += 2;
                height += 2;
            }

            Grid = new DisplayChar?[width, height];

            foreach (XmlNode node in areaXml.ChildNodes)
            {
                if (node.Name == "empty_char")
                {
                    EmptyChar = new DisplayChar { Char = node.Attributes["char"].Value.ToCharArray()[0], Colour = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), node.Attributes["color"].Value) };
                    break;
                }
            }

            Focus = false;
            
            
            EntryPoints = new Dictionary<string, int[]>();

            XmlNode entryPointsXml = null;

            foreach (XmlNode node in areaXml.ChildNodes)
            {
                if (node.Name == "entry_points")
                {
                    entryPointsXml = node;
                }
            }

            if (entryPointsXml == null)
            {
                throw new ArgumentException("The xml provided for this area dosen't contain an entry_points tag.");
            }

            foreach (XmlNode point in entryPointsXml)
            {
                AddEntryPoint(point.Attributes["name"].Value, new int[2] { Convert.ToInt32(point.Attributes["x"].Value), Convert.ToInt32(point.Attributes["y"].Value) });
            }
            
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    if (Border == true && (i == 0 || i == Grid.GetLength(0) - 1 || j == 0 || j == Grid.GetLength(1) - 1))
                    {
                        Grid[i, j] = new DisplayChar { Char = 'B', Colour = ConsoleColor.Green };
                    }
                    else
                    {
                        Grid[i, j] = null;
                    }
                }
            }

            Entities = new Dictionary<string, IEntity>();

            XmlNode xmlEntites = null;

            foreach (XmlNode node in areaXml.ChildNodes)
            {
                if (node.Name == "entities")
                {
                    xmlEntites = node;
                }
            }

            if (xmlEntites == null)
            {
                throw new ArgumentException("The xml provided for this area dosen't contain an entites tag.");
            }

            foreach (XmlNode entity in xmlEntites.ChildNodes)
            {
                AddEntity((IEntity)Activator.CreateInstance(entities.GetNestedType(entity.Attributes["type"].Value), entity, entities, methodsClass), new int[2] { Convert.ToInt32(entity.Attributes["start_x"].Value), Convert.ToInt32(entity.Attributes["start_y"].Value) });
            }
        }

        public virtual object Clone()
        {
            // also check update to entity cloning

            return new Area(this);
        }

        public bool AddEntity(IEntity entity, int[] location)
        {
            if (Entities.ContainsKey(entity.Name))
            {
                throw new ArgumentException("An entity with the same name allready exists.");
            }

            int[] trueLocation = CorrectLocation((int[])location.Clone());

            if (Grid[trueLocation[0], trueLocation[1]] == null)
            {
                Grid[trueLocation[0], trueLocation[1]] = entity.Indicator;

                if (Focus == true)
                {
                    UpdateScreen(new List<int[]> { trueLocation });
                }

                entity.SetLocation(location);

                EntityCollision += entity.Collision;

                if (entity is IMobile)
                {
                    ((IMobile)entity).MoveEntity += Move;
                    ((IMobile)entity).RespawnEntity += Move;
                }

                entity.GetProximity += GetProximity;

                Entities[entity.Name] = entity;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddEntryPoint(string name, int[] location)
        {
            EntryPoints[name] = location;

            return true;
        }

        public static DisplayChar?[,] FormGrid(XmlNode gridData, int width, int height)
        {
            DisplayChar?[,] grid = new DisplayChar?[width, height];

            int y = 0;

            foreach (XmlNode row in gridData.ChildNodes)
            {
                if (row.Name == "row")
                {
                    int x = 0;
                    foreach (XmlNode Item in row.ChildNodes)
                    {
                        if (Item.Name == "display_char")
                        {
                            grid[x, y] = new DisplayChar { Char = Item.Attributes["char"].Value.ToCharArray()[0], Colour = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Item.Attributes["color"].Value) };
                        }
                        else if (Item.Name == "Null")
                        {
                            grid[x, y] = null;
                        }

                        x++;
                    }
                }

                y++;
            }

            return grid;
        }

        public bool ShowArea(IPlayer player, Dictionary<ConsoleKey, Responce> playerActions, string entryPoint = "deafult")
        {
            if (EntryPoints == null)
            {
                throw new InvalidOperationException("No entry points have been defined. An area must have at least one entry point defined.");
            }
            if (EntryPoints.ContainsKey("entryPoint"))
            {
                throw new InvalidOperationException("The entry point provided dosen't exist.");
            }
            if (Entities.ContainsKey(player.Name))
            {
                throw new ArgumentException("An entity with the same name allready exists.");
            }

            // Add the player entity
            PlayerName = player.Name;
            AddEntity(player, EntryPoints[entryPoint]);

            // Ready the console window
            Console.Clear();
            Console.WindowWidth = Grid.GetLength(0);
            Console.WindowHeight = Grid.GetLength(1);

            if (Console.WindowWidth < 40)
            {
                Console.WindowWidth = 40;
            }

            if (Console.WindowHeight < 20)
            {
                Console.WindowHeight = 20;
            }

            // Make the area the focus
            Focus = true;

            // Make this area's entities active
            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                entity.Value.Active = true;
            }

            // Draw the area on the console
            List<int[]> positions = new List<int[]>();
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    positions.Add(new int[2] { i, j });
                }
            }
            UpdateScreen(positions);

            // Create the player and NPC threads
            ControllPlayerThread = new System.Threading.Thread(new System.Threading.ThreadStart(((IPlayer)Entities[PlayerName]).ControllPlayer));
            
            ControllNPCThreads = new List<System.Threading.Thread>();
            
            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                if (entity.Value is INpc)
                {
                    ControllNPCThreads.Add(new System.Threading.Thread(new System.Threading.ThreadStart(((INpc)entity.Value).ControllEntity)));
                }
            }

            // Start the threads
            ControllPlayerThread.Start();

            foreach (System.Threading.Thread thread in ControllNPCThreads)
            {
                thread.IsBackground = true;// Kills the threads when the player thread terminates
                thread.Start();
            }

            return true;
        }

        public bool HideArea()
        {
            // Cause threads to terminate and prevent any further event handeling
            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                entity.Value.Active = false;
            }

            Focus = false;

            int[] playerLocation = CorrectLocation(Entities[PlayerName].Location);

            Grid[playerLocation[0], playerLocation[1]] = null;

            ((IPlayer)Entities[PlayerName]).MoveEntity -= Move;
            EntityCollision -= ((IPlayer)Entities[PlayerName]).Collision;

            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                EntityCollision -= entity.Value.Collision;

                if (entity.Value is IMobile)
                {
                    ((IMobile)entity.Value).MoveEntity -= Move;
                }
            }

            //Entities[PlayerName]. // Delete player copy here??
            Entities[PlayerName] = null;

            Entities.Remove(PlayerName);

            return true;
        }

        public virtual bool UpdateScreen(List<int[]> positions)
        {
            foreach (int[] position in positions)
            {
                Console.SetCursorPosition(position[0], position[1]);

                if (Grid[position[0], position[1]] != null)
                {
                    Console.ForegroundColor = ((DisplayChar)Grid[position[0], position[1]]).Colour;
                    Console.Write(((DisplayChar)Grid[position[0], position[1]]).Char);
                }
                else
                {
                    Console.ForegroundColor = EmptyChar.Colour;
                    Console.Write(EmptyChar.Char);
                }
            }

            return true;
        }

        public bool Swap(int[] l1, int[] l2)
        {
            int[] location1 = CorrectLocation(l1);
            int[] location2 = CorrectLocation(l2);

            DisplayChar? temp = Grid[location1[0], location1[1]];
            Grid[location1[0], location1[1]] = Grid[location2[0], location2[1]];
            Grid[location2[0], location2[1]] = temp;

            if (Focus == true)
            {
                UpdateScreen(new List<int[]> { location1, location2 });
            }

            return true;
        }

        public bool Move(object entity, MoveEntityEventArgs e)
        {
            int[] location1 = CorrectLocation((int[])e.Location.Clone());
            int[] location2 = CorrectLocation((int[])e.NewLocation.Clone());

            if (location2[0] < 0 || location2[0] >= Grid.GetLength(0) || location2[1] < 0 || location2[1] >= Grid.GetLength(1))
            {
                return false;
            }

            if (Grid[location2[0], location2[1]] != null)
            {
                // Find object
                foreach (KeyValuePair<string, IEntity> otherEntity in Entities)
                {
                    if (otherEntity.Value.Location[0] == e.NewLocation[0]  && otherEntity.Value.Location[1] == e.NewLocation[1])
                    {
                        // Raise collision event
                        OnRaiseEntityCollision(new EntityCollisionEventArgs((IEntity)entity, otherEntity.Value));
                        break;
                    }
                }

                return false;
            }

            DisplayChar? temp = Grid[location1[0], location1[1]];
            Grid[location1[0], location1[1]] = null;
            Grid[location2[0], location2[1]] = temp;

            if (Focus == true)
            {
                UpdateScreen(new List<int[]> { location1, location2 });
            }

            return true;
        }

        public Tuple<bool, int[]> Move(object entity, RespawnEntityEventArgs e)
        {
            int[] location1 = CorrectLocation((int[])e.Location.Clone());
            int[] newLocation = (int[])(EntryPoints[e.EntryPoint].Clone());
            int[] location2 = CorrectLocation(newLocation);

            if (location2[0] < 0 || location2[0] >= Grid.GetLength(0) || location2[1] < 0 || location2[1] >= Grid.GetLength(1))
            {
                return new Tuple<bool, int[]>(false, (int[])e.Location.Clone());
            }

            if (Grid[location2[0], location2[1]] != null)
            {
                // Find object
                foreach (KeyValuePair<string, IEntity> otherEntity in Entities)
                {
                    if (otherEntity.Value.Location[0] == newLocation[0] && otherEntity.Value.Location[1] == newLocation[1])
                    {
                        // Raise collision event
                        OnRaiseEntityCollision(new EntityCollisionEventArgs((IEntity)entity, otherEntity.Value));
                        break;
                    }
                }
                
                return new Tuple<bool, int[]>(false, (int[])e.Location.Clone());
            }

            DisplayChar? temp = Grid[location1[0], location1[1]];
            Grid[location1[0], location1[1]] = null;
            Grid[location2[0], location2[1]] = temp;

            if (Focus == true)
            {
                UpdateScreen(new List<int[]> { location1, location2 });
            }

            return new Tuple<bool, int[]>(true, newLocation);
        }

        public Dictionary<string, int[]> GetProximity(object sender, GetProximityEventArgs e)
        {
            Dictionary<string, int[]> nearbyEntities = new Dictionary<string, int[]>();

            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                if (entity.Value.Location[0] >= e.Location[0] - e.XDistance && entity.Value.Location[0] <= e.Location[0] + e.XDistance && entity.Value.Location[1] >= e.Location[1] - e.YDistance && entity.Value.Location[1] <= e.Location[1] + e.YDistance)
                {
                    nearbyEntities[entity.Key] = (int[])entity.Value.Location.Clone();
                }
            }

            return nearbyEntities;
        }

        public int GetWidth()
        {
            int width = Grid.GetLength(0);

            if (Border == true)
            {
                width -= 2;
            }

            return width;
        }

        public int GetHeight()
        {
            int height = Grid.GetLength(1);

            if (Border == true)
            {
                height -= 2;
            }

            return height;
        }

        protected int[] CorrectLocation(int[] location)
        {
            int[] alteredLocation = new int[2];
            alteredLocation[0] = location[0];
            alteredLocation[1] = location[1];

            if (Border == true)
            {
                alteredLocation[0] += 1;
                alteredLocation[1] += 1;
            }

            return alteredLocation;
        }


        public event EntityCollisionEventHandler EntityCollision;

        protected virtual void OnRaiseEntityCollision(EntityCollisionEventArgs e)
        {
            EntityCollision?.Invoke(this, e);
        }
    }
}
