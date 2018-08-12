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
    public interface ICombatable: IInteractable
    {
    //- Properties
        double HP { get; }



        //- Methods
        bool TryAttack(double dammage);

        bool Attacked(object sender, AttackedEventArgs e);



        //- Events
        event AttackEventHandler Attack;
    }


    public delegate IEntity AttackEventHandler(object sender, AttackEventArgs e);

    public class AttackEventArgs : EventArgs
    {
        public int[] AtLocation { get; private set; }

        public double RawDammage { get; private set; }

        public AttackEventArgs(int[] location, double rawDammage)
        {
            AtLocation = location;

            RawDammage = rawDammage;
        }
    }
}