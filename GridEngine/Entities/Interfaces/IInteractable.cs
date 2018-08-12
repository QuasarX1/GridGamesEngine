// Common and project usings
using System;
using System.Collections.Generic;
using System.Text;

// Addidional dependancy usings
using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

// Custom usings

namespace GridEngine.Entities
{
    public interface IInteractable: IEntity
    {
    //- Methods
        bool TryInteract();// Only raise event if object is an IInteractable

        bool InteractWith(object sender, EntityInteractionEventArgs e);

        

    //- Events
        event GetAtLocationEventHandler GetAtLocation;

        event EntityInteractionEventHandler Interact;
    }



    public delegate IEntity GetAtLocationEventHandler(object sender, GetAtLocationEventArgs e);

    public class GetAtLocationEventArgs : EventArgs
    {
        public int[] AtLocation { get; private set; }

        public GetAtLocationEventArgs(int[] location)
        {
            AtLocation = location;
        }
    }


    public delegate void EntityInteractionEventHandler(object sender, EntityInteractionEventArgs e);

    public class EntityInteractionEventArgs : EventArgs
    {
        public IInteractable Interacter { get; private set; }
        public IInteractable Target { get; private set; }

        public EntityInteractionEventArgs(IInteractable interacter, IInteractable target)
        {
            Interacter = interacter;
            Target = target;
        }
    }
}