using AssertUtils;
using AStar;
using System.Diagnostics;
using System.Numerics;
using Wc3_Combat_Game.Components.Controllers.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Components.Controllers
{
    class BasicAIController: IUnitController
    {
        public Point[]? Path = null;
        public int nextWayPoint = 0;

        // Store the target's position when the path was last calculated
        private Vector2 _lastTargetPosition = Vector2.Zero;
        // Threshold for target movement to trigger path recalculation
        private const float TargetRecalculateThresholdSqr = 25f; // If target moves more than 5 units squared

        public void Update(Unit unit, float deltaTime, IBoardContext context)
        {
            // Example: move toward nearest enemy
            //Unit target = FindNearestEnemy(unit);
            if(!unit.IsAlive)
            {
                unit.MoveSpeed *= 0.95f;
            }
            Unit? target = unit.Target;
            if(unit.IsAlive && target != null)
            {
                float distSqrt = unit.DistanceSquaredTo(target);
                if(unit.Weapon != null)
                {
                    if(distSqrt <= unit.Weapon.AttackRangeSqr
                        && context.Map?.HasLineOfSight(context.Map.ToGrid(unit.Position), context.Map.ToGrid(target.Position)) == true)
                    {
                        unit.Weapon.TryShootEntity(unit, target, context);
                        return;
                    }
                }

                Vector2 targetPos = target.Position;

                // Pathfinding logic
                if(context.PathFinder != null && context.Map != null)
                {
                    // Recalculate path if no valid path exists OR
                    // if the target has moved significantly since the last path calculation
                    if(!ValidPath() || Vector2.DistanceSquared(targetPos, _lastTargetPosition) > TargetRecalculateThresholdSqr)
                    {
                        Pathfind(unit, targetPos, context);
                        // Assert that a valid path was found. In a release build, this might be handled differently
                        // (e.g., by stopping movement or finding a new target).
                        if (!ValidPath())
                        {
                            Pathfind(unit, targetPos, context); // Do it again for debug tracking purposes

                        }
                        AssertUtil.Assert(() => ValidPath(), "Pathfinding failed to find a valid path.");
                    }

                    if(ValidPath())
                    {
                        // Shortcutting: Find the furthest visible waypoint
                        int currentWaypointIndex = nextWayPoint;
                        for(int i = nextWayPoint; i < Path!.Length; i++)
                        {
                            // Check if the unit has line of sight to this waypoint
                            // and if it's a valid tile to move towards (e.g., not blocked)
                            if(context.Map.HasLineOfSight(context.Map.ToGrid(unit.Position), Path[i]))
                            {
                                currentWaypointIndex = i; // This waypoint is visible, try to go to it
                            }
                            else
                            {
                                // If we lose LOS to the current waypoint, stop looking further
                                break;
                            }
                        }

                        targetPos = context.Map.FromGrid(Path[currentWaypointIndex]);

                        // If the unit is close enough to the current waypoint, advance to the next one
                        if(Vector2.DistanceSquared(targetPos, unit.Position) <= 4f && nextWayPoint < Path.Length - 1) // 4f is 2 units squared
                        {
                            nextWayPoint++; // Move to the next waypoint
                            targetPos = context.Map.FromGrid(Path[nextWayPoint]); // Update targetPos to the new waypoint
                        }
                        // If we are at the last waypoint and close enough, clear the path
                        else if(Vector2.DistanceSquared(targetPos, unit.Position) <= 4f && nextWayPoint == Path.Length - 1)
                        {
                            Path = null; // Reached the end of the path
                            nextWayPoint = 0;
                            unit.TargetPoint = unit.Position; // Stop moving
                            return; // Path completed, no further movement needed this frame
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
                        _lastTargetPosition = targetPos; // Update the last known target position.

                    }
                }

                // Calculate steering force towards the target position (waypoint or direct target)
                Vector2 steeringTarget = GetPartialSteeringTarget(unit, targetPos, context);

                // Anti-bunching: Add separation force from nearby friendly units
                // This assumes IBoardContext can provide nearby units. You might need to implement
                // a method like GetNearbyUnits(unit.Position, separationRadius) in IBoardContext.
                // For demonstration, let's assume context.GetAllUnits() exists and we filter.
                Vector2 separationForce = Vector2.Zero;
                
                IEnumerable<Unit> entities = context.Entities.Entities
                    .OfType<Unit>()
                    .Where(u => u.IsAlive && u.Team.IsFriendlyTo(unit.Team))
                    .ToList(); // Get all friendly units

                separationForce = GetSeparationSteering(unit, entities, context);
                separationForce *= 0.25f;

                // Combine steering forces (path following + separation).
                // You might want to weight these forces based on priority.
                Vector2 combinedSteering = Vector2.Normalize(steeringTarget + separationForce);


                if(combinedSteering != Vector2.Zero) // Ensure we have a direction to move
                {
                    Vector2 dir = combinedSteering * unit.MoveSpeed * deltaTime; // Multiply by deltaTime for frame-rate independence
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

        /// <summary>
        /// Checks if a valid path currently exists and if the next waypoint is within bounds.
        /// </summary>
        /// <returns>True if a valid path exists, false otherwise.</returns>
        public bool ValidPath()
        {
            return Path != null && Path.Length > 0 && nextWayPoint < Path.Length;
        }

        /// <summary>
        /// Finds a path from the unit's current position to the target position using the board's pathfinder.
        /// </summary>
        /// <param name="unit">The unit for which to find the path.</param>
        /// <param name="targetPos">The target world position.</param>
        /// <param name="context">The board context providing map and pathfinder.</param>
        public void Pathfind(Unit unit, Vector2 targetPos, IBoardContext context)
        {
            Map? map = context.Map;
            PathFinder? pathFinder = context.PathFinder;
            AssertUtil.NotNull(map);
            AssertUtil.NotNull(pathFinder);
            Point startTile = map.ToGrid(unit.Position);
            Point targetTile = map.ToGrid(targetPos);
            Debug.Assert(map[startTile].IsWalkable, "Start tile is not walkable");
            Debug.Assert(map[targetTile].IsWalkable, "Target tile is not walkable");
            Path = pathFinder.FindPath(startTile, targetTile);
            if(Path.Length > 0)
            {
                foreach(Point point in Path)
                {
                    Debug.Assert(map[point].IsWalkable, $"Path contains non-walkable tile at {point}");
                }
                nextWayPoint = 0;

                if(Path[0] == startTile && Path.Length > 1)
                {
                    nextWayPoint = 1; // Skip first waypoint if it's the current tile.
                }

            }
            else
            {
                Map.ValidateMapAndPathfinder(map, pathFinder); // Make sure its valid.
            }
        }

        /// <summary>
        /// Calculates a steering vector towards a target position, applying shortcutting if line of sight is clear.
        /// </summary>
        /// <param name="myPos">The current position of the unit.</param>
        /// <param name="targetPos">The target world position.</param>
        /// <param name="context">The board context providing map information.</param>
        /// <returns>A normalized steering vector, or Vector2.Zero if already at target.</returns>
         public static Vector2 GetPartialSteeringTarget(Unit unit, Vector2 targetPos, IBoardContext context)
        {

            Map? map = context.Map;
            AssertUtil.NotNull(map);
            if(unit.Position == targetPos)
            {
                return Vector2.Zero; // Already at the target position.
            }

            Point myTile = map.ToGrid(unit.Position);
            Point targetTile = map.ToGrid(targetPos);

            if(map.HasLineOfSight(unit.Position, targetPos,unit.Size))
            {
                // Direct line
                return Vector2.Normalize(targetPos - unit.Position);
            }

            Vector2 bestDir = Vector2.Zero;
            float bestDist = float.MaxValue;

            foreach(Point neighbor in map.GetAdjacentTiles(myTile))
            {
                //Vector2Int offset = neighbor - myTile;
                if(!map[neighbor].IsWalkable)
                    continue;

                float dist = GeometryUtils.DistanceSquared(targetTile, neighbor);
                if(dist < bestDist)
                {
                    bestDist = dist;
                    bestDir = Vector2.Normalize(map.FromGrid(neighbor) - unit.Position);
                }
            }
            AssertUtil.Assert(() => !bestDir.IsNaN());
            return bestDir;
        }
        /// <summary>
        /// Calculates a steering force to push the unit away from nearby friendly units (anti-bunching).
        /// </summary>
        /// <param name="unit">The current unit.</param>
        /// <param name="allUnits">A collection of all units in the game (or at least nearby units).</param>
        /// <param name="context">The board context.</param>
        /// <returns>A vector representing the separation force.</returns>
        private Vector2 GetSeparationSteering(Unit unit, IEnumerable<Unit> allUnits, IBoardContext context)
        {
            Vector2 separationForce = Vector2.Zero;
            float separationRadius = unit.Size * 2; // Adjust this radius as needed
            float separationRadiusSqr = separationRadius * separationRadius;

            foreach(Unit otherUnit in allUnits)
            {
                // Only consider other units that are alive, are allies, and are not the unit itself
                if(otherUnit.IsAlive && otherUnit != unit && otherUnit.Team.IsFriendlyTo(unit.Team))
                {
                    float distSqr = unit.DistanceSquaredTo(otherUnit);

                    // If too close, apply a repulsive force
                    if(distSqr > 0 && distSqr < separationRadiusSqr)
                    {
                        // Calculate a force inversely proportional to the distance
                        Vector2 awayFromOther = Vector2.Normalize(unit.Position - otherUnit.Position);
                        // The closer the units, the stronger the force
                        separationForce += awayFromOther * (1.0f - (distSqr / separationRadiusSqr));
                    }
                }
            }
            return separationForce;
        }

        /// <summary>
        /// Draws debug information for the AI controller, such as the path.
        /// Only active in DEBUG builds.
        /// </summary>
        /// <param name="g">The Graphics object for drawing.</param>
        /// <param name="context">The draw context providing map information.</param>
        /// <param name="unit">The unit associated with this controller.</param>
        public void DrawDebug(Graphics g, IDrawContext context, Unit unit)
        {
#if DEBUG

            if(!unit.IsAlive) return;
            Map? map = context.Map;
            AssertUtil.NotNull(map);

            //[DebugSetting.DrawEnemyControllerState] = false,       
            //[DebugSetting.DrawEnemyControllerFullPath] = false,    
            //[DebugSetting.DrawEnemyControllerNextWaypoint] = false,
            //[DebugSetting.DrawEnemyControllerLOS] = false,         

            if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerState))
            {
                // Really need to make a state diagram. This isn't super helpful yet.
                // Since it always just says Moving for now. But eh. 1 thing at a time.
                string stateText = $"AI: {(ValidPath() ? "Moving" : "Idle")}";
                using var font = new Font("Arial", 8);
                using var brush = new SolidBrush(Color.White);
                g.DrawString(stateText, font, brush, unit.Position.X + unit.Size, unit.Position.Y - unit.Size);
            }

            if(ValidPath())
            {
                AssertUtil.NotNull(Path); // ValidPath already does this, but compiler insists.
                float tileSize = map.TileSize / 4;
                int x = nextWayPoint;
                Vector2 currentPointWorld = (x == nextWayPoint) ? unit.Position : map.FromGrid(Path[x-1]);
                Vector2 nextPointWorld = map.FromGrid(Path[x]);
               
                
                if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerNextWaypoint))
                {
                    // Next waypoint and line connecting it.
                    g.DrawLine(Pens.Yellow, currentPointWorld.ToPoint(), nextPointWorld.ToPoint());
                    Vector2 currentTargetWaypointWorld = map.FromGrid(Path![nextWayPoint]);
                    tileSize = map.TileSize / 3; // Make it slightly larger
                    g.FillRectangle(Brushes.Red, currentTargetWaypointWorld.X - tileSize / 2, currentTargetWaypointWorld.Y - tileSize / 2, tileSize, tileSize);

                }

                if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerFullPath))
                {
                    // Draw lines between path points and highlight waypoints
                    for(x = nextWayPoint; x < Path.Length; x++) // Start drawing from the current waypoint
                    {
                        currentPointWorld = (x == nextWayPoint) ? unit.Position : map.FromGrid(Path[x - 1]);
                        nextPointWorld = map.FromGrid(Path[x]);


                        // Draw the line segment
                        g.DrawLine(Pens.Yellow, currentPointWorld.ToPoint(), nextPointWorld.ToPoint());

                        // Draw a rectangle at each waypoint
                        g.FillRectangle(Brushes.Yellow, nextPointWorld.X - tileSize / 2, nextPointWorld.Y - tileSize / 2, tileSize, tileSize);
                    }
                }
                if (context.DebugSettings.Get(DebugSetting.DrawEnemyControllerLOS))
                {
                    // Draw line of sight Check to next waypoint, via asking the map to draw it.
                    map.DrawDebugLineOfSight(g, unit.Position, nextPointWorld, unit.Size);
                }
            }
#endif
        }
    }
}
