using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using static Wc3_Combat_Game.GameConstants;

namespace Wc3_Combat_Game
{
    public class Player(Vector2 size, Vector2 position, Brush brush) : Entity(size, position, brush)
    {
        Vector2 _velocity = Vector2.Zero;
        internal void InputMove(Vector2 move)
        {
            _velocity = move;
        }

        public override void Update()
        {
            _position += _velocity * FixedDeltaTime;
            _velocity = Vector2.Zero; // No inertia for now
        }
    }
}
