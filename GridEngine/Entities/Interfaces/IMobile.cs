using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
using GridEngine.Enums;
//using GridEngine.Structures;

namespace GridEngine.Entities
{
    public interface IMobile: IEntity
    {
    //- Properties
        Direction Facing { get; }



    //- Property control methods
        int[] GetAheadLoc();
        int[] GetBehindLoc();
        int[] GetLeftLoc();
        int[] GetRightLoc();


    //- Operation methods
        bool Move(int[] newLocation);

        bool Respawn(string entryPoint);

        bool MoveRelitive(int[] vector);

        bool MoveLeft(int spaces = 1);

        bool MoveRight(int spaces = 1);

        bool MoveUp(int spaces = 1);

        bool MoveDown(int spaces = 1);

        void TurnLeft(int noTimes = 1);

        void TurnRight(int noTimes = 1);

        bool MoveRelitiveAhead(int spaces = 1);

        bool MoveRelitiveLeft(int spaces = 1);

        bool MoveRelitiveRight(int spaces = 1);

        bool MoveRelitiveBehind(int spaces = 1);

        bool MoveFacingUp(int spaces = 1);

        bool MoveFacingDown(int spaces = 1);

        bool MoveFacingLeft(int spaces = 1);

        bool MoveFacingRight(int spaces = 1);



    //- Events
        event MoveEventHandler MoveEntity;

        event RespawnEventHandler RespawnEntity;
    }

    public delegate bool MoveEventHandler(object sender, MoveEntityEventArgs e);

    public class MoveEntityEventArgs : EventArgs
    {
        public int[] Location { get; private set; }
        public int[] NewLocation { get; private set; }

        public MoveEntityEventArgs(int[] location, int[] newLocation)
        {
            Location = location;
            NewLocation = newLocation;
        }
    }

    public delegate Tuple<bool, int[]> RespawnEventHandler(object sender, RespawnEntityEventArgs e);

    public class RespawnEntityEventArgs : EventArgs
    {
        public int[] Location { get; private set; }
        public string EntryPoint { get; private set; }

        public RespawnEntityEventArgs(int[] location, string entryPont)
        {
            Location = location;
            EntryPoint = entryPont;
        }
    }
}
