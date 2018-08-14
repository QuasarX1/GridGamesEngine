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



    //- Property control methods
        bool Attacked(object sender, AttackedEventArgs e);

        void TryUpdateHP(object sender, UpdateHPEventArgs e);


    //- Operation methods
        bool TryAttack(double dammage);



    //- Events
        event AttackEventHandler Attack;

        event UpdateHPEventHandler UpdateHP;
    }


    public delegate bool AttackEventHandler(object sender, AttackEventArgs e);

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


    public delegate void UpdateHPEventHandler(object sender, UpdateHPEventArgs e);

    public class UpdateHPEventArgs : EventArgs
    {
        public double Ammount { get; private set; }

        public double Multyplyer { get; private set; }

        public UpdateHPEventArgs(double ammount, double multyplyer = 1)
        {
            Ammount = ammount;

            Multyplyer = multyplyer;
        }
    }
}