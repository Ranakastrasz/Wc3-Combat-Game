using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Components.Controllers.Interface;
using AssertUtils;
using AStar;
using Wc3_Combat_Game.Util;
using System.ComponentModel;
using System.Diagnostics;

namespace Wc3_Combat_Game.Components.Controllers
{
    class BasicAIController: IUnitController
    {
        public Point[]? Path = null;
        public int nextWayPoint = 0;

        public void Update(Unit unit, float deltaTime, IBoardContext context)
        {
            // Example: move toward nearest enemy
            //Unit target = FindNearestEnemy(unit);
            if(!unit.IsAlive)
            {
                unit.MoveSpeed *= 0.95f;
            }
            Unit? target = unit.Target;
            if(unit.IsAlive && target != null )
            {
                float distSqrt = unit.DistanceSquaredTo(target);
                if(unit.Weapon != null)
                {
                    if(distSqrt <= unit.Weapon.GetAttackRangeSqr())
                    {
                        unit.Weapon.TryShootEntity(unit, target, context);
                        return;
                    }
                }
                Vector2 targetPos = target.Position;
                if(context.PathFinder != null)
                {
                    //if(ValidPath())
                    if(false) // For the moment, just gonna always recalculate the path.
                    {
                        AssertUtil.NotNull(Path);
                        AssertUtil.NotNull(context.Map); // Already certain, valid path would be impossible otherwise.

                        targetPos = context.Map.FromGrid(Path[nextWayPoint]);

                        if(Vector2.DistanceSquared(targetPos, unit.Position) <= 4f) // Close enough to the waypoint.
                        {
                            // For now, until I figure out a better way, It will just make a new Pathfind call.
                            //Pathfind(unit, target.Position, context);
                            //AssertUtil.Assert(() => ValidPath(), "Pathfinding failed to find a valid path after reaching waypoint.");

                            targetPos = context.Map.FromGrid(Path![nextWayPoint]);
                            //nextWayPoint++;
                            //if(nextWayPoint >= Path.Length)
                            //{
                            //    // Kill the pathfinding, we reached the end.
                            //    Path = null;
                            //    nextWayPoint = 0;
                            //    unit.TargetPoint = unit.Position; // Stop moving.
                            //}
                        }

                    }
                    else
                    {
                        Pathfind(unit, target.Position, context);
                        AssertUtil.Assert(() => ValidPath(), "Pathfinding failed to find a valid path.");
                        // Strictly speaking, I don't think this is a good way to do this.
                        // Failure may be valid in some cases.

                        AssertUtil.NotNull(context.Map);
                        targetPos = context.Map.FromGrid(Path![nextWayPoint]);

                    }
                }

                Vector2 SteeringTarget = GetPartialSteeringTarget(unit.Position, targetPos, context);
                if(SteeringTarget != unit.Position)
                {
                    Vector2 dir = SteeringTarget * unit.MoveSpeed;
                    unit.TargetPoint = unit.Position + dir;
                }
                else
                {
                    unit.TargetPoint = unit.Position; // No movement needed.
                }
            }
            else
            {
                // No target, Keep doing what you're doing.
                // Well, ideally, follow the path properly, but That should really be a method call.
            }
        }
        public bool ValidPath()
        {
            return Path != null && Path.Length > 0 && nextWayPoint < Path.Length;
        }

        public void Pathfind(Unit unit, Vector2 targetPos, IBoardContext context)
        {
            Map? map = context.Map;
            PathFinder? pathFinder = context.PathFinder;
            AssertUtil.NotNull(map);
            AssertUtil.NotNull(pathFinder);
            Point startTile = map.ToGrid(unit.Position);
            Point targetTile = map.ToGrid(targetPos);
            Path = pathFinder.FindPath(startTile, targetTile);
            if(Path.Length > 0)
            {
                nextWayPoint = 0;

                if(Path[0] == startTile && Path.Length > 1)
                {
                    nextWayPoint = 1; // Skip first waypoint if it's the current tile.
                }

            }
        }

        public static Vector2 GetPartialSteeringTarget(Vector2 myPos, Vector2 targetPos, IBoardContext context)
        {

            Map? map = context.Map;
            AssertUtil.NotNull(map);
            if(myPos == targetPos)
            {
                return Vector2.Zero; // Already at the target position.
            }

            Point myTile = map.ToGrid(myPos);
            Point targetTile = map.ToGrid(targetPos);

            if(map.HasLineOfSight(myTile, targetTile))
            {
                // Direct line
                return Vector2.Normalize(targetPos - myPos);
            }

            Vector2 bestDir = Vector2.Zero;
            float bestDist = float.MaxValue;

            foreach(Point neighbor in map.GetAdjacentTiles(myTile))
            {
                //Vector2Int offset = neighbor - myTile;
                if(!map[neighbor].IsWalkable)
                    continue;

                float dist = GeometryUtils.DistanceToSquared(targetTile, neighbor);
                if(dist < bestDist)
                {
                    bestDist = dist;
                    bestDir = Vector2.Normalize(map.FromGrid(neighbor) - myPos);
                }
            }
            AssertUtil.Assert(() => !bestDir.IsNaN());
            return bestDir;
        }
        public void DrawDebug(Graphics g, IDrawContext context, Unit unit)
        {
#if DEBUG
           
            if(!unit.IsAlive) return;  
            Map? map = context.Map;  
            AssertUtil.NotNull(map);  
            if (Path != null && Path.Length > 0)  
            {  
                for(int x = 0; x < Path.Length; x++)  
                {  
                    Vector2 pos = (x == 0) ? unit.Position : map.FromGrid(Path[x-1]);  
                    Vector2 nextPos = map.FromGrid(Path[x]);  
                    float size = map.TileSize / 4;  
                    g.DrawLine(Pens.Yellow, pos.ToPoint(), nextPos.ToPoint());  
                    g.FillRectangle(Brushes.Yellow, pos.X-size/2, pos.Y - size / 2, size, size);  
                    // Need to draw lines between them too.  
                }  
            }  
#endif
        }
    }
}
