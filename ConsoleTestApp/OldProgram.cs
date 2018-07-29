//using System;
//using System.Collections.Generic;

//using System.Xml;

//using ConsoleGridEngine;
//using ConsoleGridEngine.Areas;
//using ConsoleGridEngine.Deligates;
//using ConsoleGridEngine.Engine;
//using ConsoleGridEngine.Entities;
//using ConsoleGridEngine.Structures;

//namespace ConsoleTestApp.OldProgram
//{
//    class Program
//    {
//        static Engine engine;

//        public void Main(string[] args)
//        {
//            XmlDocument xmlDocument = new XmlDocument();

//            xmlDocument.Load("..\\..\\..\\TestGameXML.xml");

//            engine = new Engine(xmlDocument, typeof(Methods), typeof(Entities));



//            //// Create areas
//            //Dictionary<string, IArea> areas = new Dictionary<string, IArea>();




//            //XmlDocument xmlDocument = new XmlDocument();

//            //xmlDocument.Load("..\\..\\..\\TestGridXML.xml");

//            //IArea area = null;

//            //if (xmlDocument.DocumentElement.FirstChild.Name == "Grid")
//            //{

//            //}
//            //else if (xmlDocument.DocumentElement.FirstChild.Name == "Background")
//            //{
//            //    DisplayChar?[,] backgroundGrid = new DisplayChar?[Convert.ToInt32(xmlDocument.DocumentElement.Attributes["width"].Value), Convert.ToInt32(xmlDocument.DocumentElement.Attributes["height"].Value)];

//            //    int x = 0;

//            //    foreach (XmlNode row in xmlDocument.DocumentElement.FirstChild.ChildNodes)
//            //    {
//            //        if (row.Name == "Row")
//            //        {
//            //            int y = 0;

//            //            foreach (XmlNode Item in row.ChildNodes)
//            //            {
//            //                if (Item.Name == "DisplayChar")
//            //                {
//            //                    backgroundGrid[x, y] = new DisplayChar { Char = Item.Attributes["char"].Value.ToCharArray()[0], Colour = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Item.Attributes["color"].Value) };
//            //                }
//            //                else if (Item.Name == "Null")
//            //                {
//            //                    backgroundGrid[x, y] = null;
//            //                }

//            //                y++;
//            //            }
//            //        }

//            //        x++;
//            //    }

//            //    area = (IArea)Activator.CreateInstance(Type.GetType(xmlDocument.DocumentElement.Name), backgroundGrid, Convert.ToBoolean(xmlDocument.DocumentElement.Attributes["border"].Value), null);

//            //    //area = new BackgroundArea(backgroundGrid, Convert.ToBoolean(xmlDocument.DocumentElement.Attributes["border"].Value), null);
//            //}

//            //areas["Start"] = area;


//            ////areas["Start"] = new Area(18, 9, border: true);

//            ////var bg = new DisplayChar?[36,18];
//            ////for (int i = 0; i < bg.GetLength(0); i++)
//            ////{
//            ////    for (int j = 0; j < bg.GetLength(1); j++)
//            ////    {
//            ////        bg[i, j] = new DisplayChar { Char = '.', Colour = ConsoleColor.Yellow };
//            ////    }
//            ////}
//            ////areas["Start"] = new BackgroundArea(bg, border: true);

//            //areas["Start"].AddEntryPoint("deafult", new int[2] { 0, 0 });

//            //// Add static entities (e.g. walls)
//            //areas["Start"].AddEntity(new Wall("Wall1"), new int[2] { 5, 5 });
//            //areas["Start"].AddEntity(new Wall("Wall2"), new int[2] { 5, 6 });

//            //// Add non-player entities
//            ////Entity Enemy1 = new Entity("Enemy1", new DisplayChar { Char = 'I', Colour = ConsoleColor.DarkGreen });
//            //areas["Start"].AddEntity(new Enemy("Enemy1"), new int[2] { 9, 4 });

//            ////Entity tree1 = new Entity("Tree1", new DisplayChar { Char = 'T', Colour = ConsoleColor.DarkGreen });
//            ////areas["Start"].AddEntity(tree1, new int[2] { 4, 4 });
//            ////Entity tree2 = new Entity("Tree2", new DisplayChar { Char = 'T', Colour = ConsoleColor.DarkGreen });
//            ////areas["Start"].AddEntity(tree2, new int[2] { 6, 5 });
//            ////Entity tree3 = new Entity("Tree3", new DisplayChar { Char = 'T', Colour = ConsoleColor.DarkGreen });
//            ////areas["Start"].AddEntity(tree3, new int[2] { 8, 2 });

//            ////for (int i = 1; i < areas["Start"].GetWidth(); i++)
//            ////{
//            ////    areas["Start"].AddEntity(new Wall("Wall" + i), new int[2] { i, 6 });
//            ////}

//            ////Create engine with areas
//            //engine = new Engine("My Game", ref areas);

//            //// Add player entity
//            //engine.AddPlayer("Player");

//            //// Add collision actions
//            //engine.Player.AddCollisionAction(typeof(Enemy), new ColisionResponce(Respawn));
//            //engine.Player.AddCollisionAction(typeof(Wall), new ColisionResponce(NextArea));

//            //// Add player controll actions
//            //engine.KeyAction(ConsoleKey.W, new Responce(Up));
//            //engine.KeyAction(ConsoleKey.A, new Responce(Left));
//            //engine.KeyAction(ConsoleKey.S, new Responce(Down));
//            //engine.KeyAction(ConsoleKey.D, new Responce(Right));

//            // Start the engine
//            engine.Start();
//        }

//        //static bool? Up(IPlayer entity)
//        //{
//        //    return entity.MoveUp();
//        //}

//        //static bool? Down(IPlayer entity)
//        //{
//        //    return entity.MoveDown();
//        //}

//        //static bool? Left(IPlayer entity)
//        //{
//        //    return entity.MoveLeft();
//        //}

//        //static bool? Right(IPlayer entity)
//        //{
//        //    return entity.MoveRight();
//        //}

//        //static void Respawn(IEntity entity)
//        //{
//        //    ((IMobile)entity).Move(new int[2] { 0, 0 });
//        //}

//        //static void NextArea(IEntity entity)
//        //{
//        //    engine.ChangeArea("Start", "deafult");
//        //}

//        public static class Methods
//        {
//            static bool? Up(IPlayer entity)
//            {
//                return entity.MoveUp();
//            }

//            static bool? Down(IPlayer entity)
//            {
//                return entity.MoveDown();
//            }

//            static bool? Left(IPlayer entity)
//            {
//                return entity.MoveLeft();
//            }

//            static bool? Right(IPlayer entity)
//            {
//                return entity.MoveRight();
//            }

//            static void Respawn(IEntity entity)
//            {
//                ((IMobile)entity).Move(new int[2] { 0, 0 });
//            }

//            static void NextArea(IEntity entity)
//            {
//                engine.ChangeArea("Start", "deafult");
//            }
//        }

//        public static class Entities
//        {
//            public class Player : ConsoleEngine_NetCore2.Entities.PlayerEntity
//            {
//                public Player(XmlNode playerXml, Dictionary<ConsoleKey, Responce> actions, Type entities, Type methodsClass) : base(playerXml, actions, entities, methodsClass) { }
//            }

//            public class Wall : Entity, IEntity
//            {
//                public Wall(string name) : base(name, new DisplayChar { Char = 'W', Colour = ConsoleColor.Gray })
//                {

//                }

//                public Wall(XmlNode entityXml, Type entities, Type methodsClass) : base(entityXml, entities, methodsClass)
//                {

//                }
//            }

//            public class Enemy : NPC
//            {
//                public Enemy(string name) : base(name, new DisplayChar { Char = 'I', Colour = ConsoleColor.Red }) { }

//                public override void Action()
//                {
//                    Dictionary<string, int[]> nearbyEntities = OnRaiseGetProximity(new GetProximityEventArgs(Location, 10, 10));

//                    int[] moveDirection = null;

//                    foreach (KeyValuePair<string, int[]> entity in nearbyEntities)
//                    {
//                        if (entity.Key == "Player")
//                        {
//                            moveDirection = new int[2] { entity.Value[0] - Location[0], entity.Value[1] - Location[1] };
//                        }
//                    }

//                    if (moveDirection != null)
//                    {
//                        if (Math.Abs(moveDirection[0]) >= Math.Abs(moveDirection[1]))
//                        {
//                            if (moveDirection[0] <= 0)
//                            {
//                                MoveLeft();
//                            }
//                            else
//                            {
//                                MoveRight();
//                            }
//                        }
//                        else
//                        {
//                            if (moveDirection[1] <= 0)
//                            {
//                                MoveUp();
//                            }
//                            else
//                            {
//                                MoveDown();
//                            }
//                        }
//                    }


//                    //bool result;

//                    //if (direction == 1)
//                    //{
//                    //    result = MoveLeft();
//                    //}
//                    //else if (direction == 2)
//                    //{
//                    //    result = MoveUp();
//                    //}
//                    //else if (direction == 3)
//                    //{
//                    //    result = MoveRight();
//                    //}
//                    //else
//                    //{
//                    //    result = MoveDown();
//                    //}

//                    //if (result == false)
//                    //{
//                    //    direction++;
//                    //}
//                    //if (direction == 5)
//                    //{
//                    //    direction = 1;
//                    //}
//                }
//            }
//        }
//    }
//}
