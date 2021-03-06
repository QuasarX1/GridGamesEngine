﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using GridEngine.Deligates;
using GridEngine.Entities;

namespace GridEngine.Areas
{
    public class LayeredArea: BackgroundArea, ILayeredArea
    {
        public LayeredArea(XmlNode areaXml, IGameHost host) : base(areaXml, host)
        {

        }

        public LayeredArea(LayeredArea area): base(area)
        {

        }
        
        public override object Clone()
        {
            return new LayeredArea(this);
        }
    }
}
