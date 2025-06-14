using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Components.Controllers.Interface;
using AssertUtils;
using AStar;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Components.Controllers
{
    class BasicAIController : IUnitController
    {
        //private PathFinder _pathFinder;


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

        public void RegisterGrid(Map map)
        {
            //_pathFinder = new PathFinder(map.PathfinderGrid);
        }
        
        //public void Pathfind(Unit unit, Vector2 targetPos, IBoardContext context)
        //{
        //    Map? map = context.Map;
        //    AssertUtil.AssertNotNull(map);
        //    Vector2Int startTile = map.ToGrid(unit.Position);
        //    Vector2Int targetTile = map.ToGrid(targetPos);
        //    Point[] path = _pathFinder.FindPath(startTile.ToPoint(), targetTile.ToPoint());
        //    if (path.Length > 0)
        //    {
        //        // Move to the first tile in the path
        //        Vector2 nextTilePos = map.FromGrid(path[0]);
        //        Vector2 moveDir = Vector2.Normalize(nextTilePos - unit.Position);
        //        unit.OrderMove(moveDir * unit.MoveSpeed);
        //    }
        //}

        public static Vector2 GetPartialSteeringTarget(Vector2 myPos, Vector2 targetPos, IBoardContext context)
        {

            Map? map = context.Map;
            AssertUtil.AssertNotNull(map);

            Point myTile = map.ToGrid(myPos);
            Point targetTile = map.ToGrid(targetPos);
            
            if (map.HasLineOfSight(myTile, targetTile))
            {
                // Direct line
                return Vector2.Normalize(targetPos - myPos);
            }
            
            Vector2 bestDir = Vector2.Zero;
            float bestDist = float.MaxValue;

            foreach (Point neighbor in map.GetAdjacentTiles(myTile))
            {
                //Vector2Int offset = neighbor - myTile;
                if (!map[neighbor].IsWalkable)
                    continue;
            
                float dist = GeometryUtils.DistanceToSquared(targetTile, neighbor);
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
