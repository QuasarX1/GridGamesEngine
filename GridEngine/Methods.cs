using System;
using System.Collections.Generic;
using System.Text;

using GridEngine.Areas;
using GridEngine.Deligates;
using GridEngine.Entities;
using GridEngine.Enums;
//using GridEngine.Structures;
using static GridEngine.Enums.InputKeys;

namespace GridEngine
{
    public static class Methods
    {
        public static bool? Up(IMobile entity, params string[] args)
        {
            return entity.MoveUp(Convert.ToInt16(args[0]));
        }

        public static bool? Down(IMobile entity, params string[] args)
        {
            return entity.MoveDown(Convert.ToInt16(args[0]));
        }

        public static bool? Left(IMobile entity, params string[] args)
        {
            return entity.MoveLeft(Convert.ToInt16(args[0]));
        }

        public static bool? Right(IMobile entity, params string[] args)
        {
            return entity.MoveRight(Convert.ToInt16(args[0]));
        }

        public static bool? Respawn(IMobile entity, params string[] args)
        {
            return entity.Respawn(args[0]);
        }

        public static bool? CheckFront(IInteractable entity, params string[] args)
        {
            return entity.CheckFront();
        }

        public static bool? Attack(IInteractable entity, params string[] args)
        {
            return entity.Attack();
        }
    }
}