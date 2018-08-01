using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using GridEngine.Deligates;
using GridEngine.Entities;
using GridEngine.Structures;

namespace GridEngine.Areas
{
    public class BackgroundArea: Area, IArea
    {
        public DisplayChar?[,] Background { get; protected set; }

        public BackgroundArea(DisplayChar?[,] background, bool border = false, DisplayChar? emptyChar = null) : base(background.GetLength(0), background.GetLength(1), border, emptyChar)
        {
            Background = background;
        }

        public BackgroundArea(BackgroundArea area): base(area)
        {
            Background = new DisplayChar?[area.Background.GetLength(0),area.Background.GetLength(1)];

            for (int i = 0; i < Background.GetLength(0); i++)
            {
                for (int j = 0; j < Background.GetLength(1); j++)
                {
                    Background[i, j] = area.Background[i, j];
                }
            }
        }

        public BackgroundArea(XmlNode areaXml, Type entities, Type methodsClass) : base(areaXml, entities, methodsClass)
        {
            XmlNode backgroundXml = null;

            foreach (XmlNode node in areaXml.ChildNodes)
            {
                if (node.Name == "background")
                {
                    backgroundXml = node;
                    break;
                }
            }

            if (backgroundXml == null)
            {
                throw new ArgumentException("The xml provided for this area dosen't contain a background tag.");
            }

            Background = FormGrid(backgroundXml, Convert.ToInt32(areaXml.Attributes["width"].Value), Convert.ToInt32(areaXml.Attributes["height"].Value));
        }

        public override object Clone()
        {
            return new BackgroundArea(this);
        }

        public override bool UpdateDisplay(List<int[]> positions)
        {
            foreach (int[] position in positions)
            {
                Console.SetCursorPosition(position[0], position[1]);

                if (Grid[position[0], position[1]] != null)
                {
                    Console.ForegroundColor = ((DisplayChar)Grid[position[0], position[1]]).Colour;
                    Console.Write(((DisplayChar)Grid[position[0], position[1]]).Char);
                }
                else if (Background[position[0] - ((Border == true)? 1: 0), position[1] - ((Border == true) ? 1 : 0)] != null)
                {
                    Console.ForegroundColor = ((DisplayChar)Background[position[0] - ((Border == true) ? 1 : 0), position[1] - ((Border == true) ? 1 : 0)]).Colour;
                    Console.Write(((DisplayChar)Background[position[0] - ((Border == true) ? 1 : 0), position[1] - ((Border == true) ? 1 : 0)]).Char);
                }
                else
                {
                    Console.ForegroundColor = EmptyChar.Colour;
                    Console.Write(EmptyChar.Char);
                }
            }

            return true;
        }
    }
}