using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Interface.Controllers
{
    class IPlayerController : IUnitController
    {
        private InputManager _input;

        public IPlayerController(InputManager input)
        {
            _input = input;
        }

        public void Update(Unit unit, float deltaTime, BoardContext context)
        {
            if (!unit.IsAlive) return;

            Vector2 move = Vector2.Zero;
            if (_input.IsKeyDown(Keys.W)) move.Y -= 1;
            if (_input.IsKeyDown(Keys.S)) move.Y += 1;
            if (_input.IsKeyDown(Keys.A)) move.X -= 1;
            if (_input.IsKeyDown(Keys.D)) move.X += 1;

            if (move != Vector2.Zero)
                unit.Move(move);


            if (unit.Weapon != null)
            {
                if (_input.IsMouseClicked())
                    unit.Weapon.TryShootPoint(unit, _input.MouseClickedPosition, context);

                else if (_input.IsMouseDown())
                    unit.Weapon.TryShootPoint(unit, _input.CurrentMousePosition, context);
            }
        }

    }
}
