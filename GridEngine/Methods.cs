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

        public static bool? TurnLeft(IMobile entity, params string[] args)
        {
            entity.TurnLeft(Convert.ToInt16(args[0]));

            return true;
        }

        public static bool? TurnRight(IMobile entity, params string[] args)
        {
            entity.TurnRight(Convert.ToInt16(args[0]));

            return true;
        }

        public static bool? MoveFacingUp(IMobile entity, params string[] args)
        {
            entity.MoveFacingUp(Convert.ToInt16(args[0]));

            return true;
        }

        public static bool? MoveFacingDown(IMobile entity, params string[] args)
        {
            entity.MoveFacingDown(Convert.ToInt16(args[0]));

            return true;
        }

        public static bool? MoveFacingLeft(IMobile entity, params string[] args)
        {
            entity.MoveFacingLeft(Convert.ToInt16(args[0]));

            return true;
        }

        public static bool? MoveFacingRight(IMobile entity, params string[] args)
        {
            entity.MoveFacingRight(Convert.ToInt16(args[0]));

            return true;
        }

        public static bool? Respawn(IMobile entity, params string[] args)
        {
            return entity.Respawn(args[0]);
        }

        public static bool? Interact(IInteractable entity, params string[] args)
        {
            return entity.Interact();
        }

        public static bool? Attack(IInteractable entity, params string[] args)
        {
            return entity.Attack();
        }
    }
}