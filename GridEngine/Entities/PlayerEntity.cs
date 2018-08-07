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
        public Dictionary<Keys, Tuple<Responce, string[]>> Actions { get; protected set; }


        //public PlayerEntity(string name, Dictionary<Keys, Responce> actions, DisplayChar? indicator = null) : base(name, indicator)
        //{
        //    Actions = actions;
        //}

        public PlayerEntity(IPlayer player, Dictionary<Keys, Tuple<Responce, string[]>> actions) : base((MobileEntity)player)
        {
            Actions = actions;
        }

        public PlayerEntity(PlayerEntity player): base(player)
        {
            this.Actions = player.Actions;
            this.StopEngine = player.StopEngine;
        }
        
        public PlayerEntity(XmlNode playerXml, Dictionary<Keys, Tuple<Responce, string[]>> actions) : base(playerXml)
        {
            Actions = actions;
        }
        
        public override object Clone()
        {
            return new PlayerEntity(this);

            //return new PlayerEntity(Name, Actions, Indicator);
        }

        public void ControllPlayer()
        {
            while (Active == true)
            {
                // Handle key input - on event? - start and stop?
                Keys key = Console.ReadKey(true).Key;

                bool? result;

                if (Actions.ContainsKey(key))
                {
                    result = Actions[key].Item1(this, Actions[key].Item2);

                    if (result == false)
                    {
                        Console.Beep();
                        System.Threading.Thread.Sleep(200);
                    }
                    else if (result == null)
                    {
                        break;
                    }
                }
                else
                {
                    Console.Beep();
                }

                // Clear keyboard buffer
                while (Console.KeyAvailable == true)
                {
                    Console.ReadKey(true);
                }

                //System.Threading.Thread.Sleep(250);
            }

            if (Active == true)
            {
                OnRaiseEndEvent(null);
            }
        }

        public event StopEngineEventHandler StopEngine;

        protected virtual void OnRaiseEndEvent(StopEngineEventArgs e)
        {
            StopEngine?.Invoke(this, e);
        }
    }
}