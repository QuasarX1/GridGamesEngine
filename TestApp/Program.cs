using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.Reflection;
using System.IO;

using ConsoleGridEngine;
using static QuasarCode.IO.StreamOperations;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.xml"); //"*.gamedata.xml");
            string[] files = Directory.GetDirectories(Environment.CurrentDirectory);

            int gameIndex;

            int nextIndex = 0;

            // Select game
            while (true)
            {
                List<string> options = new List<string>(9);

                for (int i = 0; i < 8; i++)
                {
                    if (files.Length > nextIndex + i)
                    {
                        options.Add(Path.GetFileName(files[nextIndex + i]));
                    }
                    else
                    {
                        options.Capacity = i + 1;
                        break;
                    }
                }
                options.Add("Next Page");

                Console.Clear();
                int selection = Option(options, "Select the game.");

                if (selection != options.Count - 1)
                {
                    gameIndex = nextIndex + selection;
                    break;
                }
                else
                {
                    nextIndex += 8;

                    if (nextIndex > files.Length - 1)
                    {
                        nextIndex = 0;
                    }
                }
            }

            // Load game data - only one class in each dll defining all other types
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(files[gameIndex] + "\\" + Path.GetFileName(files[gameIndex]) + "_gamedata.xml");

            Type[] entitiesArray;
            Type[] methodsArray;

            Dictionary<string, Type> entities = new Dictionary<string, Type>();
            Dictionary<string, Type> methods = new Dictionary<string, Type>();

            entitiesArray = Assembly.LoadFrom(files[gameIndex] + "\\" + Path.GetFileName(files[gameIndex]) + "_entities.dll").GetTypes();

            methodsArray = Assembly.LoadFrom(files[gameIndex] + "\\" + Path.GetFileName(files[gameIndex]) + "_methods.dll").GetTypes();

            foreach (Type type in entitiesArray)
            {
                entities[type.Name] = type;
            }

            foreach (Type type in methodsArray)
            {
                methods[type.Name] = type;
            }

            ConsoleGridEngine.Engine.Engine engine = new ConsoleGridEngine.Engine.Engine(xmlDocument, methodsArray[0], entitiesArray[0]);

            engine.Start();
        }
    }
}
