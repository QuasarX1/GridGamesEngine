using System;
using System.Collections.Generic;
using System.Text;

using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Entities
{
    public interface IMobile: IEntity
    {
        event MoveEventHandler MoveEntity;

        event RespawnEventHandler RespawnEntity;

        bool Move(int[] newLocation);

        bool Respawn(string entryPoint);

        bool MoveRelitive(int[] vector);

        bool MoveLeft(int spaces = 1);

        bool MoveRight(int spaces = 1);

        bool MoveUp(int spaces = 1);

        bool MoveDown(int spaces = 1);
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
