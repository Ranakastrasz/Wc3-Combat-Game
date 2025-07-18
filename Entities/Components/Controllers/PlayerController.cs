﻿using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Entities.Components.Controllers
{
    class PlayerController: IUnitController
    {
        private InputManager _input;

        public PlayerController(InputManager input)
        {
            _input = input;
        }

        public void Update(Unit unit, float deltaTime, IBoardContext context)
        {
            if(!unit.IsAlive) return;

            Vector2 move = Vector2.Zero;
            if(_input.IsKeyDown(Keys.W)) move.Y -= 1;
            if(_input.IsKeyDown(Keys.S)) move.Y += 1;
            if(_input.IsKeyDown(Keys.A)) move.X -= 1;
            if(_input.IsKeyDown(Keys.D)) move.X += 1;

            if(move != Vector2.Zero)
                // Wrong. Normalizes, but when diagonal, continues moving further on a single keystroke.
                unit.TargetPoint = unit.Position + move * (unit.MoveSpeed * deltaTime);



            if(unit.Abilities != null)
            {
                if(_input.IsMouseClicked())
                    unit.Abilities[0].TryTargetPoint(unit, _input.MouseClickedPosition, context);

                else if(_input.IsMouseDown())
                    unit.Abilities[0].TryTargetPoint(unit, _input.CurrentMousePosition, context);
            }
        }
        public void DrawDebug(Graphics g, IDrawContext context, Unit unit)
        {
            //throw new NotImplementedException();
            // No idea what goes here yet
            // Probably draw the current input and desired movement direction?
            if(!unit.IsAlive) return;

        }
    }
}
