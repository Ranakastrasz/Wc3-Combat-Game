using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Util.UnitConversion;

using static Wc3_Combat_Game.Util.UnitConversion.GameSpace;

namespace Wc3_Combat_Game.Entities.Projectiles
{
    //public class ProjectileMover: IMoveable
    //{
    //    private WorldVector2 _position;
    //    private Vector2 _velocity;
    //    public Vector2 Velocity { get => _velocity; set => _velocity = value; }
    //    public WorldVector2 Position { get => _position; }
    //
    //    public ProjectileMover(WorldVector2 position, Vector2 velocity)
    //    { 
    //        Velocity = velocity;
    //        _position = position;
    //        
    //    }
    //
    //    public void Teleport(Vector2 position, IBoardContext context)
    //    {
    //        _position = new WorldVector2(position);
    //    }
    //
    //    public void Update(Entity owner, float deltaTime, IBoardContext context)
    //    {
    //        _position = new WorldVector2(_position.Value + _velocity * deltaTime);
    //    }
    //}
}