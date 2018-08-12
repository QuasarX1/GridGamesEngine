using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Entities
{
    public interface IPlayer: IMobile, ICombatable
    {
    //- Fields and Properties
        Dictionary<Keys, Tuple<Responce, string[]>> Actions { get; }



    //- Operation methods
        void OnKeyPressed(Keys key);



    //- Events
        event StopEngineEventHandler StopEngine;
    }

    public delegate void StopEngineEventHandler(object sender, StopEngineEventArgs e);

    public class StopEngineEventArgs : EventArgs {  }
}
