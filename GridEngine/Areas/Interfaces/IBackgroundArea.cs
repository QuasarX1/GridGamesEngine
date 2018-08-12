using System;
using System.Collections.Generic;
using System.Text;

namespace GridEngine.Areas
{
    public partial interface IBackgroundArea: IArea
    {
    //- Fields and Properties
        string[,] Background { get; }

        new string[,] DisplayGrid { get; }
    }
}
