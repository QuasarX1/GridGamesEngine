using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Areas
{
    public interface IArea: ICloneable
    {
        event EntityCollisionEventHandler EntityCollision;

        System.Threading.Thread ControllPlayerThread { get; }

        List<System.Threading.Thread> ControllNPCThreads { get; }

        string[,] Grid { get; }

        //bool Border { get; }

        Dictionary<string, int[]> EntryPoints { get; }

        string EmptySpaceImage { get; }

        bool Focus { get; }

        Dictionary<string, IEntity> Entities { get; }

        string PlayerName { get; }

        
        bool AddEntity(IEntity entity, int[] location);

        bool AddEntryPoint(string name, int[] location);

        bool ShowArea(IPlayer player, string entryPoint = "deafult");// Dictionary<Keys, Responce> playerActions, string entryPoint = "deafult");

        bool Pause();

        bool Resume();

        bool HideArea();

        bool UpdateDisplay(List<Tuple<int[], int[]>> updates);

        bool Move(object entity, MoveEntityEventArgs e);

        int GetWidth();

        int GetHeight();

        event KeyPressEventHandler KeyPress;
        void OnRaiseKeyPress(KeyPressEventArgs e);
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
}
