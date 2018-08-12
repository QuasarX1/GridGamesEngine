using System;
using System.Collections.Generic;
using System.Xml;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;

namespace GridEngine.Entities
{
    public abstract class Entity: IEntity
    {
    //- Fields and Properties
        public Dictionary<Tuple<string, object>, Tuple<ColisionResponce, string[]>> CollisionActions { get; protected set; }

        public string Name { get; protected set; }

        public int ID { get; protected set; }

        public int[] Location { get; protected set; }

        public string Image { get; protected set; }

        public bool Active { get; set; }
        

        
    //- Constructors
        public Entity(XmlNode entityXml)
        {
            CollisionActions = new Dictionary<Tuple<string, object>, Tuple<ColisionResponce, string[]>>();
            
            // Set the name
            Name = entityXml.Attributes["name"].Value;

            // Set the id
            Name = entityXml.Attributes["id"].Value;

            // Load the image name
            XmlNode entitySubNode = entityXml.FirstChild;// Selects the image node

            Image = entitySubNode.Attributes["name"].Value;

            // Load the collision actions
            entitySubNode = entitySubNode.NextSibling;// Selects the collisions node

            foreach (XmlNode collisionNode in entitySubNode.ChildNodes)
            {
                List<string> stringArgs = new List<string>();

                foreach (XmlNode stringNode in collisionNode.ChildNodes)
                {
                    stringArgs.Add(stringNode.Attributes["data"].Value);
                }

                if (collisionNode.Name == "type_collision")
                {
                    Type entityType = Type.GetType("GridEngine.Entities." + collisionNode.Attributes["type"].Value);
                    
                    AddCollisionAction(entityType, new Tuple<ColisionResponce, string[]>((ColisionResponce)Delegate.CreateDelegate(typeof(ColisionResponce), typeof(Methods).GetMethod(collisionNode.Attributes["method"].Value)), stringArgs.ToArray()));
                }
                else if (collisionNode.Name == "name_collision")
                {
                    string entityName = collisionNode.Attributes["name"].Value;

                    AddCollisionAction(entityName, new Tuple<ColisionResponce, string[]>((ColisionResponce)Delegate.CreateDelegate(typeof(ColisionResponce), typeof(Methods).GetMethod(collisionNode.Attributes["method"].Value)), stringArgs.ToArray()));
                }

                else if (collisionNode.Name == "id_collision")
                {
                    int entityId = Convert.ToInt16(collisionNode.Attributes["name"].Value);

                    AddCollisionAction(entityId, new Tuple<ColisionResponce, string[]>((ColisionResponce)Delegate.CreateDelegate(typeof(ColisionResponce), typeof(Methods).GetMethod(collisionNode.Attributes["method"].Value)), stringArgs.ToArray()));
                }
            }
        }

        public Entity(Entity entity)
        {
            CollisionActions = new Dictionary<Tuple<string, object>, Tuple<ColisionResponce, string[]>>();


            Name = entity.Name;

            ID = entity.ID;

            Image = entity.Image;

            SetLocation(entity.Location);

            foreach (KeyValuePair<Tuple<string, object>, Tuple<ColisionResponce, string[]>> action in entity.CollisionActions)
            {
                if (action.Key.Item1 == "type")
                {
                    AddCollisionAction((Type)action.Key.Item2, action.Value);
                }
                else if (action.Key.Item1 == "name")
                {
                    AddCollisionAction((string)action.Key.Item2, action.Value);
                }
                else if (action.Key.Item1 == "id")
                {
                    AddCollisionAction((int)action.Key.Item2, action.Value);
                }
            }
        }

        public abstract object Clone();


    //- Addition methods
        public bool AddCollisionAction(Type entityType, Tuple<ColisionResponce, string[]> responce)
        {
            if (!CollisionActions.ContainsKey(new Tuple<string, object>("type", entityType)))
            {
                CollisionActions[new Tuple<string, object>("type", entityType)] = responce;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddCollisionAction(string entityName, Tuple<ColisionResponce, string[]> responce)
        {
            if (!CollisionActions.ContainsKey(new Tuple<string, object>("name", entityName)))
            {
                CollisionActions[new Tuple<string, object>("name", entityName)] = responce;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddCollisionAction(int entityId, Tuple<ColisionResponce, string[]> responce)
        {
            if (!CollisionActions.ContainsKey(new Tuple<string, object>("id", entityId)))
            {
                CollisionActions[new Tuple<string, object>("id", entityId)] = responce;
                return true;
            }
            else
            {
                return false;
            }
        }


    //- Removal methods
        public void RemoveCollisionAction(Type entityType)
        {
            if (CollisionActions.ContainsKey(new Tuple<string, object>("type", entityType)))
            {
                CollisionActions.Remove(new Tuple<string, object>("type", entityType));
            }
        }

        public void RemoveCollisionAction(string entityName)
        {
            if (CollisionActions.ContainsKey(new Tuple<string, object>("name", entityName)))
            {
                CollisionActions.Remove(new Tuple<string, object>("name", entityName));
            }
        }

        public void RemoveCollisionAction(int entityId)
        {
            if (CollisionActions.ContainsKey(new Tuple<string, object>("id", entityId)))
            {
                CollisionActions.Remove(new Tuple<string, object>("id", entityId));
            }
        }

        public bool RemoveCollisionActionOrFail(Type entityType)
        {
            if (CollisionActions.ContainsKey(new Tuple<string, object>("type", entityType)))
            {
                CollisionActions.Remove(new Tuple<string, object>("type", entityType));

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveCollisionActionOrFail(string entityName)
        {
            if (CollisionActions.ContainsKey(new Tuple<string, object>("name", entityName)))
            {
                CollisionActions.Remove(new Tuple<string, object>("name", entityName));

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveCollisionActionOrFail(int entityId)
        {
            if (CollisionActions.ContainsKey(new Tuple<string, object>("id", entityId)))
            {
                CollisionActions.Remove(new Tuple<string, object>("id", entityId));

                return true;
            }
            else
            {
                return false;
            }
        }


    //- Updating methods
        public void OnCollision(IEntity otherEntity, IArea area)
        {
            foreach (KeyValuePair<Tuple<string, object>, Tuple<ColisionResponce, string[]>> CollisionAction in CollisionActions)
            {
                if (CollisionAction.Key.Item1 == "type")
                {
                    if (otherEntity.GetType() == (Type)CollisionAction.Key.Item2)
                    {
                        CollisionAction.Value.Item1(this, CollisionAction.Value.Item2);

                        break;
                    }
                }
                else if (CollisionAction.Key.Item1 == "name")
                {
                    if (otherEntity.Name == (string)CollisionAction.Key.Item2)
                    {
                        CollisionAction.Value.Item1(this, CollisionAction.Value.Item2);

                        break;
                    }
                }
                else if (CollisionAction.Key.Item1 == "id")
                {
                    if (otherEntity.ID == (int)CollisionAction.Key.Item2)
                    {
                        CollisionAction.Value.Item1(this, CollisionAction.Value.Item2);

                        break;
                    }
                }

            }
        }


    //- Property control methods
        public void SetLocation(int[] location)
        {
            if (location != null)
            {
                Location = (int[])location.Clone();
            }
            else
            {
                Location = null;
            }
        }
        

    //- Operation methods
        public void Collision(object sender, EntityCollisionEventArgs e)
        {
            if (Active == true)
            {
                if (e.Entity1.Name == Name)
                {
                    OnCollision(e.Entity2, (IArea)sender);
                }
                else if (e.Entity2.Name == Name)
                {
                    OnCollision(e.Entity1, (IArea)sender);
                }
            }
        }



    //- Events
        public event GetProximityEventHandler GetProximity;

        protected virtual Dictionary<string, int[]> OnRaiseGetProximity(GetProximityEventArgs e)
        {
            return GetProximity?.Invoke(this, e);
        }
    }
}