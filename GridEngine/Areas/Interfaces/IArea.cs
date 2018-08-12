using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Areas
{
    public partial interface IArea: ICloneable
    {
    //- Properties
        IGameHost Host { get; }

        string[,] Grid { get; }

        string[,] DisplayGrid { get; }

        Tuple<int[], int[]> NextUpdate { get; }

        Dictionary<string, int[]> EntryPoints { get; }

        string EmptySpaceImage { get; }

        bool Focus { get; }

        Dictionary<string, IEntity> Entities { get; }

        //System.Threading.Thread ControllPlayerThread { get; }

        List<System.Threading.Thread> ControllNPCThreads { get; }

        string PlayerName { get; }



    //- Addition methods
        bool AddEntity(IEntity entity, int[] location);

        bool AddEntryPoint(string name, int[] location);


    //- Update methods
        bool UpdateDisplay(List<Tuple<int[], int[]>> updates);

        bool Move(object entity, MoveEntityEventArgs e);

        Tuple<bool, int[]> Move(object entity, RespawnEntityEventArgs e);

    //- Property control methods
        int GetWidth();

        int GetHeight();

        Dictionary<string, int[]> GetProximity(object sender, GetProximityEventArgs e);

        IEntity GetAtLocation(object sender, GetAtLocationEventArgs e);


    //- Operation methods
        bool ShowArea(IPlayer player, string entryPoint = "deafult");// Dictionary<Keys, Responce> playerActions, string entryPoint = "deafult");

        bool Pause();

        bool Resume();

        bool HideArea();



    //- Events
        event EntityCollisionEventHandler EntityCollision;

        event KeyPressEventHandler KeyPress;

        event AttackedEventHandler Attacked;
    }


    public delegate void EntityCollisionEventHandler(object sender, EntityCollisionEventArgs e);

    public class EntityCollisionEventArgs : EventArgs
    {
        public IEntity Entity1 { get; private set; }
        public IEntity Entity2 { get; private set; }

        public EntityCollisionEventArgs(IEntity entity1, IEntity entity2)
        {
            Entity1 = entity1;
            Entity2 = entity2;
        }
    }


    public delegate void KeyPressEventHandler(object sender, KeyPressEventArgs e);

    public class KeyPressEventArgs : EventArgs
    {
        public Keys Key { get; private set; }

        public KeyPressEventArgs(Keys key)
        {
            Key = key;
        }
    }


    public delegate IEntity AttackedEventHandler(object sender, AttackedEventArgs e);

    public class AttackedEventArgs : EventArgs
    {
        public ICombatable Attacker { get; private set; }
        public ICombatable Target { get; private set; }
        public double RawDammage { get; private set; }

        public AttackedEventArgs(ICombatable attacker, ICombatable target, double rawDammage)
        {
            Attacker = attacker;

            Target = target;

            RawDammage = rawDammage;
        }
    }
}
