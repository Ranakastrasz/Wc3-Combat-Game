using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;
using MathUtils;

namespace Wc3_Combat_Game.Interface.Controllers
{
    class IBasicAIController : IUnitController
    {
        public void Update(Unit unit, float deltaTime, IBoardContext context)
        {
            // Example: move toward nearest enemy
            //Unit target = FindNearestEnemy(unit);
            if (!unit.IsAlive) return;
            Unit? target = unit.Target;
            if (target != null)
            {
                float distSqrt = unit.DistanceSquaredTo(target);
                if (unit.Weapon != null)
                {
                    if (distSqrt <= unit.Weapon.GetAttackRangeSqr())
                    {
                        unit.Weapon.TryShootEntity(unit, target, context);
                        return;
                    }
                }
                Vector2 SteeringTarget = GetPartialSteeringTarget(unit.Position, target.Position, context);
                Vector2 dir = SteeringTarget * unit.MoveSpeed;
                unit.OrderMove(dir);
            }

        }


        public Vector2 GetPartialSteeringTarget(Vector2 myPos, Vector2 targetPos, IBoardContext context)
        {
            Map map = context.Map;

            Vector2Int myTile = map.ToGrid(myPos);
            Vector2Int targetTile = map.ToGrid(targetPos);
            
            if (map.HasLineOfSight(myTile, targetTile))
            {
                // Direct line
                return Vector2.Normalize(targetPos - myPos);
            }
            
            Vector2 bestDir = Vector2.Zero;
            float bestDist = float.MaxValue;

            foreach (Vector2Int neighbor in map.GetAdjacentTiles(myTile))
            {
                //Vector2Int offset = neighbor - myTile;
                if (!map[neighbor].IsWalkable)
                    continue;
            
                float dist = (targetTile - neighbor).LengthSquared();
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestDir = Vector2.Normalize(map.FromGrid(neighbor) - myPos);
                }
            }
            
            return bestDir;
        }

    }
}
