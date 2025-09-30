using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.EntityTypes;
using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Entities.Components.Controllers
{
    class PlayerController: IUnitController
    {
        private InputManager _input;

        // These things will be done differently, later.
        private Dictionary<Keys, Vector2> _movementDirections = new()
        {
            { Keys.W, new Vector2(0, -1) },
            { Keys.S, new Vector2(0, 1) },
            { Keys.A, new Vector2(-1, 0) },
            { Keys.D, new Vector2(1, 0) },
        };

        private int _leftMouseAbility = 0; // First ability is left mouse, til Input handler hook does both jobs.
        private Dictionary <int, Keys> _abilityKeys = new()
        {
            { 1, Keys.Q },
            { 2, Keys.Space },
        };

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
                unit.TargetPoint = unit.Position + move * (unit.MoveSpeed(context) * deltaTime);


            // Abilities
            for(int x = 0; x < unit.Abilities.Count; x++)
            {
                if(unit.Abilities[x] == null) continue;
                if(_abilityKeys.TryGetValue(x, out Keys key))
                {
                    if(_input.WasKeyPressedThisFrame(key))
                        unit.Abilities[x].TryTargetPoint(unit, _input.CurrentMousePosition, context);
                }
            }
            if(unit.Abilities[_leftMouseAbility] != null)
            {

                if(_input.IsMouseClicked())
                    unit.Abilities[_leftMouseAbility].TryTargetPoint(unit, _input.MouseClickedPosition, context);

                else if(_input.IsMouseDown())
                    unit.Abilities[_leftMouseAbility].TryTargetPoint(unit, _input.CurrentMousePosition, context);
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
