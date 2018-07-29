using System;
using System.Collections.Generic;
using System.Text;

using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Entities;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Areas
{
    public interface IArea: ICloneable
    {
        event EntityCollisionEventHandler EntityCollision;

        System.Threading.Thread ControllPlayerThread { get; }

        List<System.Threading.Thread> ControllNPCThreads { get; }

        DisplayChar?[,] Grid { get; }

        bool Border { get; }

        Dictionary<string, int[]> EntryPoints { get; }

        DisplayChar EmptyChar { get; }

        bool Focus { get; }

        Dictionary<string, IEntity> Entities { get; }

        string PlayerName { get; }

        
        bool AddEntity(IEntity entity, int[] location);

        bool AddEntryPoint(string name, int[] location);

        bool ShowArea(IPlayer player, Dictionary<ConsoleKey, Responce> playerActions, string entryPoint = "deafult");

        bool HideArea();

        bool UpdateScreen(List<int[]> positions);

        bool Move(object entity, MoveEntityEventArgs e);

        int GetWidth();

        int GetHeight();
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
}
