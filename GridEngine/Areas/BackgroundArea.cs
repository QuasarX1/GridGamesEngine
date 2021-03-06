﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using GridEngine.Deligates;
using GridEngine.Entities;
//using GridEngine.Structures;

namespace GridEngine.Areas
{
    public class BackgroundArea: Area, IBackgroundArea
    {
    //- Fields and Properties
        public string[,] Background { get; protected set; }

        public new string[,] DisplayGrid { get; protected set; }


    //- Constructors
        public BackgroundArea(XmlNode areaXml, IGameHost host) : base(areaXml, host)
        {
            //XmlNode backgroundXml = null;

            //foreach (XmlNode node in areaXml.ChildNodes)
            //{
            //    if (node.Name == "background")
            //    {
            //        backgroundXml = node;
            //        break;
            //    }
            //}

            //if (backgroundXml == null)
            //{
            //    throw new ArgumentException("The xml provided for this area dosen't contain a background tag.");
            //}

            Background = FormGrid(areaXml.ChildNodes.Item(1), Convert.ToInt32(areaXml.Attributes["width"].Value), Convert.ToInt32(areaXml.Attributes["height"].Value));

            DisplayGrid = new string[Convert.ToInt32(areaXml.Attributes["width"].Value), Convert.ToInt32(areaXml.Attributes["height"].Value)];

            List<Tuple<int[], int[]>> updates = new List<Tuple<int[], int[]>>();

            for (int i = 0; i < Convert.ToInt32(areaXml.Attributes["width"].Value); i++)
            {
                for (int j = 0; j < Convert.ToInt32(areaXml.Attributes["height"].Value); j++)
                {
                    updates.Add(new Tuple<int[], int[]>(new int[] { i, j }, null));
                }
            }

            UpdateDisplay(updates);
        }

        public BackgroundArea(BackgroundArea area) : base(area)
        {
            Background = new string[area.Background.GetLength(0), area.Background.GetLength(1)];

            for (int i = 0; i < Background.GetLength(0); i++)
            {
                for (int j = 0; j < Background.GetLength(1); j++)
                {
                    Background[i, j] = area.Background[i, j];
                }
            }

            DisplayGrid = new string[area.Background.GetLength(0), area.Background.GetLength(1)];
        }

        public override object Clone()
        {
            return new BackgroundArea(this);
        }

    //- Updating methods
        public override bool UpdateDisplay(List<Tuple<int[], int[]>> updates)
        {
            ((IArea)this).UpdateDisplay(updates);

            DisplayGrid = (string[,])Background.Clone();

            for (int i = 0; i < Convert.ToInt32(GetWidth()); i++)
            {
                for (int j = 0; j < Convert.ToInt32(GetHeight()); j++)
                {
                    if (Grid[i, j] != null)
                    {
                        DisplayGrid[i, j] = Grid[i, j];
                    }
                }
            }

            return true;
        }
    }
}