using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
//using GridEngine.Structures;

namespace GridEngine.Entities
{
    public interface INpc : IMobile
    {
    //- Operation methods
        void ControllEntity();

        void Action();

        // Add interaction method
        // Add looking in direction attribute and methods for rotating, changing facing, ect...
    }
}
