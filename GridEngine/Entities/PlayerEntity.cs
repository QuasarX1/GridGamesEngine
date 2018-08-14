using System;
using System.Collections.Generic;
using System.Xml;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Entities
{
    public class PlayerEntity : MobileEntity, IPlayer
    {
    //- Fields and Properties
        public Dictionary<Keys, Tuple<Responce, string[]>> Actions { get; protected set; }

        public double HP { get; protected set; }



        //- Constructors
        public PlayerEntity(XmlNode playerXml, Dictionary<Keys, Tuple<Responce, string[]>> actions) : base(playerXml)
        {
            Actions = actions;

            UpdateHP += TryUpdateHP;
        }

        public PlayerEntity(IPlayer player, Dictionary<Keys, Tuple<Responce, string[]>> actions) : base((MobileEntity)player)
        {
            Actions = actions;

            UpdateHP += TryUpdateHP;
        }

        public PlayerEntity(PlayerEntity player): base(player)
        {
            this.Actions = player.Actions;
            this.StopEngine = player.StopEngine;

            UpdateHP += TryUpdateHP;
        }
        
        public override object Clone()
        {
            return new PlayerEntity(this);

            //return new PlayerEntity(Name, Actions, Indicator);
        }


    //- Property controll methods
        public virtual bool Attacked(object sender, AttackedEventArgs e)
        {
            OnRaiseUpdateHP(new UpdateHPEventArgs(0 - e.RawDammage));

            return true;
        }

        public virtual void TryUpdateHP(object sender, UpdateHPEventArgs e)
        {
            HP += (e.Ammount * e.Multyplyer);
        }


    //- Operation methods
        public void OnKeyPressed(Keys key)
        {
            bool? result;

            if (Actions.ContainsKey(key))
            {
                result = Actions[key].Item1(this, Actions[key].Item2);

                if (result == false)
                {
                    //Console.Beep();
                    //System.Threading.Thread.Sleep(200);
                }
                else if (result == null)
                {
                    OnRaiseEndEvent(null);
                }
            }
            else
            {
                //Console.Beep();
            }
        }

        public bool TryInteract()
        {
            IEntity entity = OnRaiseGetAtLocation(new GetAtLocationEventArgs(GetAheadLoc()));

            if (entity != null && entity is IInteractable)
            {
                OnRaiseEntityInteraction(new EntityInteractionEventArgs(this, (IInteractable)entity));

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InteractWith(object sender, EntityInteractionEventArgs e)
        {
            return false;
        }

        public bool TryAttack(double dammage)
        {
            return (bool)OnRaiseAttack(new AttackEventArgs(GetAheadLoc(), dammage));
        }



    //- Events
        public event AttackEventHandler Attack;

        protected virtual bool? OnRaiseAttack(AttackEventArgs e)
        {
            return Attack?.Invoke(this, e);
        }


        public event GetAtLocationEventHandler GetAtLocation;

        protected virtual IEntity OnRaiseGetAtLocation(GetAtLocationEventArgs e)
        {
            return GetAtLocation?.Invoke(this, e);
        }


        public event EntityInteractionEventHandler Interact;

        protected virtual void OnRaiseEntityInteraction(EntityInteractionEventArgs e)
        {
            Interact?.Invoke(this, e);
        }


        public event StopEngineEventHandler StopEngine;

        protected virtual void OnRaiseEndEvent(StopEngineEventArgs e)
        {
            StopEngine?.Invoke(this, e);
        }


        public event UpdateHPEventHandler UpdateHP;

        protected virtual void OnRaiseUpdateHP(UpdateHPEventArgs e)
        {
            UpdateHP?.Invoke(this, e);
        }
    }
}