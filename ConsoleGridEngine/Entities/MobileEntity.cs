using System;
using System.Collections.Generic;
using System.Xml;

using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Entities
{
    public abstract class MobileEntity: Entity, IMobile
    {
        public MobileEntity(string name, DisplayChar? indicator = null) : base(name, indicator) { }

        private MobileEntity(IEntity entity) : base(entity.Name, entity.Indicator) { }

        public MobileEntity(MobileEntity entity) : base(entity) { }

        public MobileEntity(XmlNode entityXml, Type entities, Type methodsClass) : base(entityXml, entities, methodsClass) { }

        //public override object Clone()
        //{
        //    return new MobileEntity(this);
        //}

        public bool Move(int[] newLocation)
        {            
            if (OnRaiseMoveEntity(new MoveEntityEventArgs((int[])Location.Clone(), (int[])newLocation.Clone())) == true)
            {
                Location = newLocation;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Respawn(string entryPoint)
        {
            Tuple<bool, int[]> result = OnRaiseRespawnEntity(new RespawnEntityEventArgs((int[])Location.Clone(), entryPoint));

            if (result.Item1 == true)
            {
                Location = result.Item2;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MoveRelitive(int[] vector)
        {
            int[] newLocation = new int[2];

            newLocation[0] = Location[0] + vector[0];
            newLocation[1] = Location[1] + vector[1];

            return Move(newLocation);
        }

        public bool MoveLeft(int spaces = 1)
        {
            int[] newLocation = new int[2];

            newLocation[0] = Location[0] - spaces;
            newLocation[1] = Location[1];

            return Move(newLocation);
        }

        public bool MoveRight(int spaces = 1)
        {
            int[] newLocation = new int[2];

            newLocation[0] = Location[0] + spaces;
            newLocation[1] = Location[1];

            return Move(newLocation);
        }

        public bool MoveUp(int spaces = 1)
        {
            int[] newLocation = new int[2];

            newLocation[0] = Location[0];
            newLocation[1] = Location[1] - spaces;

            return Move(newLocation);
        }

        public bool MoveDown(int spaces = 1)
        {
            int[] newLocation = new int[2];

            newLocation[0] = Location[0];
            newLocation[1] = Location[1] + spaces;

            return Move(newLocation);
        }


        public event MoveEventHandler MoveEntity;

        protected virtual bool OnRaiseMoveEntity(MoveEntityEventArgs e)
        {
            bool? result = MoveEntity?.Invoke(this, e);

            if (result == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public event RespawnEventHandler RespawnEntity;

        protected virtual Tuple<bool, int[]> OnRaiseRespawnEntity(RespawnEntityEventArgs e)
        {
            Tuple<bool, int[]> result = RespawnEntity.Invoke(this, e);

            return result;
        }
    }
}