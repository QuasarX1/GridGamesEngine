using System;
using System.Collections.Generic;
using System.Text;

using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Entities
{
    public interface IEntity: ICloneable
    {
        event GetProximityEventHandler GetProximity;

        Dictionary<Tuple<string, object>, Tuple<ColisionResponce, string[]>> CollisionActions { get; }

        string Name { get; }

        int[] Location { get; }

        DisplayChar Indicator { get; }

        bool Active { get; set; }

        void SetLocation(int[] location);

        bool AddCollisionAction(Type entityType, Tuple<ColisionResponce, string[]> responce);

        void RemoveCollisionAction(Type entityType);

        bool RemoveCollisionActionOrFail(Type entityType);

        void Collision(object sender, EntityCollisionEventArgs e);

        void OnCollision(IEntity otherEntity, IArea area);
    }


    public delegate Dictionary<string, int[]> GetProximityEventHandler(object sender, GetProximityEventArgs e);

    public class GetProximityEventArgs : EventArgs
    {
        public int[] Location { get; private set; }
        public int XDistance { get; private set; }
        public int YDistance { get; private set; }

        public GetProximityEventArgs(int[] location, int xDistance, int yDistance)
        {
            Location = location;
            XDistance = xDistance;
            YDistance = yDistance;
        }
    }
}
