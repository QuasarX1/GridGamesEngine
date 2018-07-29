using System;
using System.Collections.Generic;
using System.Text;

using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Entities
{
    public interface IPlayer: IMobile
    {
        event StopEngineEventHandler StopEngine;

        Dictionary<ConsoleKey, Responce> Actions { get; }

        void ControllPlayer();

        // Add interaction method
        // Add looking in direction attribute and methods for rotating, changing facing, ect...
    }

    public delegate void StopEngineEventHandler(object sender, StopEngineEventArgs e);

    public class StopEngineEventArgs : EventArgs {  }
}
