using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine.Entities
{
    public interface IPlayer: IMobile, IInteractable
    {
        event StopEngineEventHandler StopEngine;

        Dictionary<Keys, Tuple<Responce, string[]>> Actions { get; }

        //void ControllPlayer();

        // Add interaction method - IInteractable

        void OnKeyPressed(Keys key);
    }

    public delegate void StopEngineEventHandler(object sender, StopEngineEventArgs e);

    public class StopEngineEventArgs : EventArgs {  }
}
