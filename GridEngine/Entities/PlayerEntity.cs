using System;
using System.Collections.Generic;
using System.Xml;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;

namespace GridEngine.Entities
{
    public class PlayerEntity : MobileEntity, IPlayer
    {
        public Dictionary<ConsoleKey, Responce> Actions { get; protected set; }

        //public PlayerEntity(string name, Dictionary<ConsoleKey, Responce> actions, DisplayChar? indicator = null) : base(name, indicator)
        //{
        //    Actions = actions;
        //}

        //public PlayerEntity(IPlayer player, Dictionary<ConsoleKey, Responce>  actions) : base(player.Name, player.Indicator)
        //{
        //    Actions = actions;
        //}

        public PlayerEntity(PlayerEntity player): base(player)
        {
            this.Actions = player.Actions;
            this.StopEngine = player.StopEngine;
        }
        
        public PlayerEntity(XmlNode playerXml, Dictionary<ConsoleKey, Responce> actions, Type entities, Type methodsClass) : base(playerXml, entities, methodsClass)
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
                ConsoleKey key = Console.ReadKey(true).Key;

                bool? result;

                if (Actions.ContainsKey(key))
                {
                    result = Actions[key](this);

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