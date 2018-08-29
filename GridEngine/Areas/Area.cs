using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Areas
{
    public class Area: IArea
    {
    //- Fields and Properties
        public IGameHost Host { get; protected set; }

        public string[,] Grid { get; protected set; }

        public string[,] DisplayGrid { get { return (string[,])Grid.Clone(); } }

        private Queue<Tuple<int[], int[]>> Updates;

        public Tuple<int[], int[]> NextUpdate { get { return Updates.Dequeue(); } }

        public Tuple<int[], int[]> PeekNextUpdate { get { return (((ICollection<Tuple<int[], int[]>>)Updates).Count != 0) ? Updates.Peek() : null; } }

        public Dictionary<string, int[]> EntryPoints { get; protected set; }

        public string EmptySpaceImage { get; protected set; }

        public bool Focus { get; protected set; }

        public Dictionary<string, IEntity> Entities { get; protected set; }

        //public System.Threading.Thread ControllPlayerThread { get; protected set; }

        public List<System.Threading.Thread> ControllNPCThreads { get; protected set; }

        public string PlayerName { get; protected set; }

        

    //- Constructors
        public Area(XmlNode areaXml, IGameHost host)
        {
            Host = host;

            int width = Convert.ToInt32(areaXml.Attributes["width"].Value);
            int height = Convert.ToInt32(areaXml.Attributes["height"].Value);

            Grid = new string[width, height];

            Updates = new Queue<Tuple<int[], int[]>>();

            foreach (XmlNode node in areaXml.ChildNodes)
            {
                if (node.Name == "empty_space_image")
                {
                    EmptySpaceImage = node.Attributes["name"].Value;
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
                    Grid[i, j] = null;
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
                AddEntity((IEntity)Activator.CreateInstance(Type.GetType("GridEngine.Entities." + entity.Attributes["type"].Value), entity), new int[2] { Convert.ToInt32(entity.Attributes["start_x"].Value), Convert.ToInt32(entity.Attributes["start_y"].Value) });
            }
        }

        public Area(IArea area)
        {
            // Duplicate the border boolean
            //Border = area.Border;

            Host = area.Host;
            
            Grid = new string[area.Grid.GetLength(0), area.Grid.GetLength(1)];

            Updates = new Queue<Tuple<int[], int[]>>();

            EmptySpaceImage = area.EmptySpaceImage;

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
        
        public virtual object Clone()
        {
            return new Area(this);
        }


    //- Addition methods
        public bool AddEntity(IEntity entity, int[] location)
        {
            if (Entities.ContainsKey(entity.Name))
            {
                throw new ArgumentException("An entity with the same name allready exists.");
            }

            if (Grid[location[0], location[1]] == null)
            {
                Grid[location[0], location[1]] = entity.Image;

                if (Focus == true)
                {
                    UpdateDisplay(new List<Tuple<int[], int[]>> { new Tuple<int[], int[]>(location, null) });
                }

                entity.SetLocation(location);

                // Subscribe to events
                EntityCollision += entity.Collision;

                entity.GetProximity += GetProximity;

                if (entity is IMobile)
                {
                    ((IMobile)entity).MoveEntity += Move;
                    ((IMobile)entity).RespawnEntity += Move;
                }

                if (entity is IInteractable)
                {
                    ((IInteractable)entity).GetAtLocation += GetAtLocation;
                    ((IInteractable)entity).Interact += (object sender, EntityInteractionEventArgs e) => { ((IInteractable)Entities[e.Target.Name]).InteractWith(sender, e); };
                }

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


    //- Updating methods
        public virtual bool UpdateDisplay(List<Tuple<int[], int[]>> updates)
        {
            foreach (Tuple<int[], int[]> update in updates)
            {
                Updates.Enqueue(update);
            }

            return true;
        }
        
        //public bool Swap(int[] l1, int[] l2)
        //{
        //    string temp = Grid[l1[0], l1[1]];
        //    Grid[l1[0], l1[1]] = Grid[l2[0], l2[1]];
        //    Grid[l2[0], l2[1]] = temp;

        //    if (Focus == true)
        //    {
        //        UpdateDisplay(new List<Tuple<int[], int[]>> { new Tuple<int[], int[]>(l1, l2) });
        //    }

        //    return true;
        //}

        public bool Move(object entity, MoveEntityEventArgs e)
        {
            int[] location1 = (int[])e.Location.Clone();
            int[] location2 = (int[])e.NewLocation.Clone();

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

            string temp = Grid[location1[0], location1[1]];
            Grid[location1[0], location1[1]] = null;
            Grid[location2[0], location2[1]] = temp;

            if (Focus == true)
            {
                UpdateDisplay(new List<Tuple<int[], int[]>> { new Tuple<int[], int[]>(location1, location2) });
            }

            return true;
        }

        public Tuple<bool, int[]> Move(object entity, RespawnEntityEventArgs e)
        {
            int[] location1 = (int[])e.Location.Clone();
            int[] newLocation = (int[])(EntryPoints[e.EntryPoint].Clone());
            int[] location2 = newLocation;

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

            string temp = Grid[location1[0], location1[1]];
            Grid[location1[0], location1[1]] = null;
            Grid[location2[0], location2[1]] = temp;

            if (Focus == true)
            {
                UpdateDisplay(new List<Tuple<int[], int[]>> { new Tuple<int[], int[]>(location1, location2) });
            }

            return new Tuple<bool, int[]>(true, newLocation);
        }


    //- Property control methods
        public int GetWidth()
        {
            int width = Grid.GetLength(0);

            return width;
        }

        public int GetHeight()
        {
            int height = Grid.GetLength(1);

            return height;
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

        public virtual IEntity GetAtLocation(object sender, GetAtLocationEventArgs e)
        {
            return Entities[(string)Grid.GetValue(e.AtLocation)];
        }


    //- Operation methods
        public bool ShowArea(IPlayer player, string entryPoint = "deafult")// Dictionary<Keys, Tuple<ColisionResponce, string[]>> playerActions, string entryPoint = "deafult")
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

            //- Add the player entity
            PlayerName = player.Name;
            AddEntity(player, EntryPoints[entryPoint]);

            //- Add subscriptions to host events
            Host.KeyPress += ((IPlayer)Entities[PlayerName]).OnKeyPressed;

            //- Make the area the focus
            Focus = true;

            //- Make this area's entities active
            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                entity.Value.Active = true;
            }

            //- Draw the area on the console
            List<Tuple<int[], int[]>> positions = new List<Tuple<int[], int[]>>();
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    positions.Add(new Tuple<int[], int[]>(new int[2] { i, j }, null));
                }
            }
            UpdateDisplay(positions);

            //- Create the player and NPC threads
            //ControllPlayerThread = new System.Threading.Thread(new System.Threading.ThreadStart(((IPlayer)Entities[PlayerName]).ControllPlayer));

            ControllNPCThreads = new List<System.Threading.Thread>();

            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                if (entity.Value is INpc)
                {
                    ControllNPCThreads.Add(new System.Threading.Thread(new System.Threading.ThreadStart(((INpc)entity.Value).ControllEntity)));
                }
            }

            //- Start the threads
            //ControllPlayerThread.Start();

            foreach (System.Threading.Thread thread in ControllNPCThreads)
            {
                thread.IsBackground = true;// Kills the threads when the player thread terminates
                thread.Start();
            }

            return true;
        }

        public bool Pause()
        {
            return true;
        }

        public bool Resume()
        {
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

            int[] playerLocation = Entities[PlayerName].Location;

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



    //- Events
        public event EntityCollisionEventHandler EntityCollision;

        protected virtual void OnRaiseEntityCollision(EntityCollisionEventArgs e)
        {
            EntityCollision?.Invoke(this, e);
        }

        public event KeyPressEventHandler KeyPress;

        protected virtual void OnRaiseKeyPress(KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }



    //- Static methods
        public static string[,] FormGrid(XmlNode gridData, int width, int height)
        {
            string[,] grid = new string[width, height];

            int y = 0;

            foreach (XmlNode row in gridData.ChildNodes)
            {
                if (row.Name == "row")
                {
                    int x = 0;
                    foreach (XmlNode Item in row.ChildNodes)
                    {
                        if (Item.Name == "Null")
                        {
                            grid[x, y] = null;
                        }
                        else
                        {
                            grid[x, y] = Item.Attributes["name"].Value;
                        }

                        x++;
                    }
                }

                y++;
            }

            return grid;
        }
    }
}
