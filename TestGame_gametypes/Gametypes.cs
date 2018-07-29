using System;
using System.Collections.Generic;
using System.Xml;
using ConsoleGridEngine.Areas;
using ConsoleGridEngine.Deligates;
using ConsoleGridEngine.Engine;
using ConsoleGridEngine.Entities;
using ConsoleGridEngine.Structures;

namespace TestGame_gametypes
{
    public static class Game
    {
        public static Engine engine;

        public static class Methods
        {
            static bool? Up(IPlayer entity)
            {
                return entity.MoveUp();
            }

            static bool? Down(IPlayer entity)
            {
                return entity.MoveDown();
            }

            static bool? Left(IPlayer entity)
            {
                return entity.MoveLeft();
            }

            static bool? Right(IPlayer entity)
            {
                return entity.MoveRight();
            }

            /// <summary>
            /// Returns the mobile entity to the specified entry point.
            /// </summary>
            /// <param name="entity">Entity that implements IMobile</param>
            /// <param name="args">A string array with one element. It must contain the name of the entry point.</param>
            static void Respawn(IEntity entity, string[] args)
            {
                //((IMobile)entity).Respawn("deafult");
                ((IMobile)entity).Respawn(args[0]);
            }

            /// <summary>
            /// Tells the engine to change to the next area
            /// </summary>
            /// <param name="entity">Entity that implements IEntity</param>
            /// <param name="args">A string array with two elements. The firt must be the area name the second is the entry point.</param>
            static void NextArea(IEntity entity, string[] args)
            {
                //engine.ChangeArea("Start", "deafult");
                engine.ChangeArea(args[0], args[1]);
            }
        }

        public static class Entities
        {
            public class Player : ConsoleGridEngine.Entities.PlayerEntity
            {
                public Player(XmlNode playerXml, Dictionary<ConsoleKey, Responce> actions, Type entities, Type methodsClass) : base(playerXml, actions, entities, methodsClass) { }
            }

            public class TP1 : Entity
            {
                public TP1(XmlNode entityXml, Type entities, Type methodsClass) : base(entityXml, entities, methodsClass) { }

                public TP1(TP1 entity) : base(entity) { }

                public override object Clone()
                {
                    return new TP1(this);
                }
            }

            public class TP2 : Entity
            {
                public TP2(XmlNode entityXml, Type entities, Type methodsClass) : base(entityXml, entities, methodsClass) { }

                public TP2(TP2 entity) : base(entity) { }

                public override object Clone()
                {
                    return new TP2(this);
                }
            }

            public class Wall : Entity, IEntity
            {
                public Wall(string name) : base(name, new DisplayChar { Char = 'W', Colour = ConsoleColor.Gray }) { }

                public Wall(XmlNode entityXml, Type entities, Type methodsClass) : base(entityXml, entities, methodsClass) { }

                public Wall(Wall entity) : base(entity) { }

                public override object Clone()
                {
                    return new Wall(this);
                }
            }
            
            public class TestEnemy : Entity, IEntity
            {
                public TestEnemy(XmlNode entityXml, Type entities, Type methodsClass) : base(entityXml, entities, methodsClass) { }

                public TestEnemy(TestEnemy entity) : base(entity) { }

                public override object Clone()
                {
                    return new TestEnemy(this);
                }
            }

            public class Enemy : NPC
            {
                public Enemy(string name) : base(name, new DisplayChar { Char = 'I', Colour = ConsoleColor.Red }) { }

                public override void Action()
                {
                    Dictionary<string, int[]> nearbyEntities = OnRaiseGetProximity(new GetProximityEventArgs(Location, 10, 10));

                    int[] moveDirection = null;

                    foreach (KeyValuePair<string, int[]> entity in nearbyEntities)
                    {
                        if (entity.Key == "Player")
                        {
                            moveDirection = new int[2] { entity.Value[0] - Location[0], entity.Value[1] - Location[1] };
                        }
                    }

                    if (moveDirection != null)
                    {
                        if (Math.Abs(moveDirection[0]) >= Math.Abs(moveDirection[1]))
                        {
                            if (moveDirection[0] <= 0)
                            {
                                MoveLeft();
                            }
                            else
                            {
                                MoveRight();
                            }
                        }
                        else
                        {
                            if (moveDirection[1] <= 0)
                            {
                                MoveUp();
                            }
                            else
                            {
                                MoveDown();
                            }
                        }
                    }
                }
            }
        }
    }
}
