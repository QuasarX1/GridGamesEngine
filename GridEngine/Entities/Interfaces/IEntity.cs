﻿using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;

namespace GridEngine.Entities
{
    public interface IEntity: ICloneable
    {
    //- Properties
        Dictionary<Tuple<string, object>, Tuple<ColisionResponce, string[]>> CollisionActions { get; }

        string Name { get; }

        int ID { get; }

        int[] Location { get; }

        string Image { get; }

        bool Active { get; set; }


    //- Addition methods
        bool AddCollisionAction(Type entityType, Tuple<ColisionResponce, string[]> responce);

        bool AddCollisionAction(string entityName, Tuple<ColisionResponce, string[]> responce);

        bool AddCollisionAction(int entityId, Tuple<ColisionResponce, string[]> responce);


    //- Removal methods
        void RemoveCollisionAction(Type entityType);

        void RemoveCollisionAction(string entityName);

        void RemoveCollisionAction(int entityId);

        bool RemoveCollisionActionOrFail(Type entityType);

        bool RemoveCollisionActionOrFail(string entityName);

        bool RemoveCollisionActionOrFail(int entityId);


    //- Updating methods
        void OnCollision(IEntity otherEntity, IArea area);


    //- Property control methods
        void SetLocation(int[] location);


    //- Operation methods
        void Collision(object sender, EntityCollisionEventArgs e);



    //- Events
        event GetProximityEventHandler GetProximity;
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
