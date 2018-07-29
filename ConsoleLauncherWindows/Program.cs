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

namespace ConsoleLauncherWindows
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                // Get the filepath to the application's data
                string folderpath = Properties.Settings.Default.Folderpath;

                if (folderpath == "" || !Directory.Exists(folderpath))
                {
                    folderpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }

                // Check application's data folder exists and create if it dosen't
                string gamesDirectory = folderpath + "\\ConsoleGridEngine";

                if (!Directory.Exists(gamesDirectory))
                {
                    Directory.CreateDirectory(gamesDirectory);
                }

                Console.Clear();
                int userChoice = Option(new List<string> { "Play Games", "Install Game", "Uninstall Game", "Change Games Directory", "Exit" }, "Main Menu");

                // Play a game
                if (userChoice == 0)
                {
                    string[] files = Directory.GetDirectories(gamesDirectory);

                    int gameIndex = SelectGame(files);

                    if (gameIndex != -1)
                    {
                        // Load game data
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(files[gameIndex] + "\\" + Path.GetFileName(files[gameIndex]) + "_gamedata.xml");

                        // Load game types
                        Assembly types = Assembly.LoadFrom(files[gameIndex] + "\\" + Path.GetFileName(files[gameIndex]) + "_gametypes.dll");

                        // Create the engine
                        types.GetType(types.GetName().Name + ".Game").GetField("engine").SetValue(null, new ConsoleGridEngine.Engine.Engine(xmlDocument, types.GetType(types.GetName().Name + ".Game").GetNestedType("Methods"), types.GetType(types.GetName().Name + ".Game").GetNestedType("Entities")));

                        // Subscribe to closing the engine
                        ((ConsoleGridEngine.Engine.Engine)types.GetType(types.GetName().Name + ".Game").GetField("engine").GetValue(null)).EngineStop += ReturnToMenu;

                        // Start the engine
                        ((ConsoleGridEngine.Engine.Engine)types.GetType(types.GetName().Name + ".Game").GetField("engine").GetValue(null)).Start();

                        break;
                    }
                }

                // Install a game
                else if (userChoice == 1)
                {
                    // Pass in filepath to folder
                    Console.Clear();

                    Console.Write("Input the full path of the game folder you wish to install.\n>>> ");
                    string gameFilePath = Console.ReadLine();

                    if (Directory.Exists(gameFilePath))
                    {
                        // Check game is valid
                        string[] gameFiles = Directory.GetFiles(gameFilePath);
                        if (((Path.GetExtension(gameFiles[0]) == ".xml" && Path.GetExtension(gameFiles[1]) == ".dll") || (Path.GetExtension(gameFiles[0]) == ".dll" && Path.GetExtension(gameFiles[1]) == ".xml")) && Directory.EnumerateFiles(gameFilePath).Any(f => f.Contains("_gamedata")) && Directory.EnumerateFiles(gameFilePath).Any(f => f.Contains("_gametypes")))
                        {
                            // Copy game folder and files to directory
                            string newGamePath = gamesDirectory + "\\" + gameFilePath.Split('\\')[gameFilePath.Split('\\').Length - 1];

                            Directory.CreateDirectory(newGamePath);

                            string[] files = Directory.GetFiles(gameFilePath);

                            foreach (string file in files)
                            {
                                string fileName = Path.GetFileName(file);
                                string destinationFile = Path.Combine(newGamePath, fileName);
                                File.Copy(file, destinationFile, false);
                            }
                        }
                    }
                }

                // Delete a game
                else if (userChoice == 2)
                {
                    string[] files = Directory.GetDirectories(gamesDirectory);
                    int gameIndex = SelectGame(files);

                    if (gameIndex != -1)
                    {
                        Directory.Delete(files[gameIndex], true);
                    }
                }

                // Change the application's directory
                else if (userChoice == 3)
                {
                    Console.Clear();

                    Console.Write("Input the full path of the folder that contains the game directory or leave blank to return to the deafult directory.\n>>> ");
                    string newFilePath = Console.ReadLine();

                    if (Directory.Exists(newFilePath) || newFilePath == "")
                    {
                        Properties.Settings.Default.Folderpath = newFilePath;
                    }

                    Properties.Settings.Default.Save();

                    gamesDirectory = newFilePath + "\\ConsoleGridEngine";

                    if (folderpath == "")
                    {
                        folderpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    }

                    if (!Directory.Exists(gamesDirectory))
                    {
                        Directory.CreateDirectory(gamesDirectory);
                    }
                }

                // Exit
                else if (userChoice == 4)
                {
                    break;
                }
            }
        }

        public static void ReturnToMenu(object sender, ConsoleGridEngine.Engine.EngineStopEventArgs e)
        {
            ((ConsoleGridEngine.Engine.Engine)sender).EngineStop -= ReturnToMenu;

            Main(new string[0]);
        }

        private static int SelectGame(string[] files)
        {
            int nextIndex = 0;

            // Select game
            while (true)
            {
                List<string> options = new List<string>(9);

                for (int i = 0; i < 7; i++)
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
                options.Add("Go Back");

                Console.Clear();
                int selection = Option(options, "Select the game.");

                if (selection == options.Count - 2)
                {
                    nextIndex += 7;

                    if (nextIndex > files.Length - 1)
                    {
                        nextIndex = 0;
                    }
                }
                else if (selection == options.Count - 1)
                {
                    return -1;
                }
                else
                {
                    return nextIndex + selection;
                }
            }
        }
    }
}
