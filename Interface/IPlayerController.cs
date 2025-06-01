using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.IO
{
    class IPlayerController : IUnitController
    {
        private InputManager _input;

        public IPlayerController(InputManager input)
        {
            _input = input;
        }

        public void Update(Unit unit, float deltaTime, float currentTime)
        {
            if (!unit.IsAlive) return;

            Vector2 move = Vector2.Zero;
            if (_input.IsKeyDown(Keys.W)) move.Y -= 1;
            if (_input.IsKeyDown(Keys.S)) move.Y += 1;
            if (_input.IsKeyDown(Keys.A)) move.X -= 1;
            if (_input.IsKeyDown(Keys.D)) move.X += 1;

            if (move != Vector2.Zero)
                move = GeometryUtils.NormalizeAndScale(move, GameConstants.PLAYER_SPEED);

            unit.Move(move);

            if (unit.Weapon != null)
            {
                if (_input.IsMouseClicked())
                    unit.Weapon.TryShoot(unit, _input.MouseClickedPosition, currentTime);

                else if (_input.IsMouseDown())
                    unit.Weapon.TryShoot(unit, _input.CurrentMousePosition, currentTime);
            }
        }

    }
}
