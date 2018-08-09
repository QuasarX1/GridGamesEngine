using System;
using System.Collections.Generic;
using System.Xml;

using GridEngine.Areas;
using GridEngine.Deligates;
using GridEngine.Enums;
//using GridEngine.Structures;

namespace GridEngine.Entities
{
    public abstract class MobileEntity: Entity, IMobile
    {
    //- Fields and Properties
        public Direction Facing { get; protected set; }



    //- Constructors
        public MobileEntity(XmlNode entityXml) : base(entityXml) { }

        //private MobileEntity(IEntity entity) : base(entity.Name, entity.Indicator) { }

        public MobileEntity(MobileEntity entity) : base(entity) { }

        
    //- Operation methods
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

        public void TurnLeft(int noTimes = 1)
        {
            Facing = (Direction)(4 % ((int)Facing) + noTimes);
        }

        public void TurnRight(int noTimes = 1)
        {
            Facing = (Direction)(4 % ((int)Facing) - noTimes);
        }

        public bool MoveRelitiveAhead(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    return MoveUp(spaces);
                case Direction.Right:
                    return MoveRight(spaces);
                case Direction.Down:
                    return MoveDown(spaces);
                case Direction.Left:
                    return MoveLeft(spaces);
                default:
                    return false;
            }
        }

        public bool MoveRelitiveLeft(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    return MoveLeft(spaces);
                case Direction.Right:
                    return MoveUp(spaces);
                case Direction.Down:
                    return MoveRight(spaces);
                case Direction.Left:
                    return MoveDown(spaces);
                default:
                    return false;
            }
        }

        public bool MoveRelitiveRight(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    return MoveRight(spaces);
                case Direction.Right:
                    return MoveDown(spaces);
                case Direction.Down:
                    return MoveLeft(spaces);
                case Direction.Left:
                    return MoveUp(spaces);
                default:
                    return false;
            }
        }

        public bool MoveRelitiveBehind(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    return MoveDown(spaces);
                case Direction.Right:
                    return MoveLeft(spaces);
                case Direction.Down:
                    return MoveUp(spaces);
                case Direction.Left:
                    return MoveRight(spaces);
                default:
                    return false;
            }
        }

        public bool MoveFacingUp(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    break;
                case Direction.Right:
                    TurnLeft();
                    break;
                case Direction.Down:
                    TurnLeft(2);
                    break;
                case Direction.Left:
                    TurnRight();
                    break;
            }

            return MoveRelitiveAhead(spaces);
        }

        public bool MoveFacingDown(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    TurnLeft(2);
                    break;
                case Direction.Right:
                    TurnRight();
                    break;
                case Direction.Down:
                    break;
                case Direction.Left:
                    TurnLeft();
                    break;
            }

            return MoveRelitiveAhead(spaces);
        }

        public bool MoveFacingLeft(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    TurnLeft();
                    break;
                case Direction.Right:
                    TurnLeft(2);
                    break;
                case Direction.Down:
                    TurnRight();
                    break;
                case Direction.Left:
                    break;
            }

            return MoveRelitiveAhead(spaces);
        }

        public bool MoveFacingRight(int spaces = 1)
        {
            switch (Facing)
            {
                case Direction.Up:
                    TurnRight();
                    break;
                case Direction.Right:
                    break;
                case Direction.Down:
                    TurnLeft();
                    break;
                case Direction.Left:
                    TurnLeft(2);
                    break;
            }

            return MoveRelitiveAhead(spaces);
        }



    //- Events
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