using System;
using System.Collections.Generic;
using System.Xml;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Entities
{
    public class PlayerEntity : MobileEntity, IPlayer// Add interaction method - IInteractable
    {
    //- Fields and Properties
        public Dictionary<Keys, Tuple<Responce, string[]>> Actions { get; protected set; }



    //- Constructors
        public PlayerEntity(XmlNode playerXml, Dictionary<Keys, Tuple<Responce, string[]>> actions) : base(playerXml)
        {
            Actions = actions;
        }

        public PlayerEntity(IPlayer player, Dictionary<Keys, Tuple<Responce, string[]>> actions) : base((MobileEntity)player)
        {
            Actions = actions;
        }

        public PlayerEntity(PlayerEntity player): base(player)
        {
            this.Actions = player.Actions;
            this.StopEngine = player.StopEngine;
        }
        
        public override object Clone()
        {
            return new PlayerEntity(this);

            //return new PlayerEntity(Name, Actions, Indicator);
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

        

    //- Events
        public event StopEngineEventHandler StopEngine;

        protected virtual void OnRaiseEndEvent(StopEngineEventArgs e)
        {
            StopEngine?.Invoke(this, e);
        }
    }
}