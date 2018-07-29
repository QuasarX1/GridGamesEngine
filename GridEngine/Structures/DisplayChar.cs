using System;
using System.Collections.Generic;

namespace GridEngine.Structures
{
    public struct DisplayChar: ICloneable
    {
        public char Char;
        public ConsoleColor Colour;

        public object Clone()
        {
            return new DisplayChar { Char = Char, Colour = Colour };
        }
    }
}
