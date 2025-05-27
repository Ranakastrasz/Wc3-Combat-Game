using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using static Wc3_Combat_Game.GameConstants;

namespace Wc3_Combat_Game
{
    public class Player : Entity
    {
        Vector2 _velocity = Vector2.Zero;

        public Player(Vector2 size, Vector2 position, Brush brush) : base(size, position, brush)
        {
        }

        internal void InputMove(Vector2 move)
        {
            _velocity = move;
        }

        public override void Update()
        {
            _position += _velocity * FIXED_DELTA_TIME;
            _velocity = Vector2.Zero; // No inertia for now
        }
    }
}
