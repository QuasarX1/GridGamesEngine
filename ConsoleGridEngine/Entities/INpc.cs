using System;
using System.Collections.Generic;
using System.Text;

using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Structures;

namespace ConsoleGridEngine.Entities
{
    public interface INpc : IMobile
    {
        void ControllEntity();

        void Action();

        // Add interaction method
        // Add looking in direction attribute and methods for rotating, changing facing, ect...
    }
}
