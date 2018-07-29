using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
using GridEngine.Structures;

namespace GridEngine.Entities
{
    public class NPC: MobileEntity, INpc
    {
        public NPC(string name, DisplayChar? indicator = null): base(name, indicator) { }

        public NPC(NPC npc) : base(npc) { }

        public override object Clone()
        {
            return new NPC(this);
        }

        public void ControllEntity()
        {
            while (Active == true)
            {
                Action();

                System.Threading.Thread.Sleep(300);
            }
        }

        public virtual void Action()
        {
            
        }
    }
}
