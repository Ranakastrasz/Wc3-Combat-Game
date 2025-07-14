using AssertUtils;
using AStar;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        public int CurrentWaypoint = 0;

        // Store the target's position when the path was last calculated
        private Vector2 _lastTargetPosition = Vector2.Zero;
        // Threshold for target movement to trigger path recalculation
        private const float TargetRecalculateThresholdSqr = 1024; // If target moves more than 32 units.

        private Vector2 _TargetPosition;
        private float _lastPathfind = 0f;

        private enum State
        {
            Idle,
            PathFinding,
            PathFollowing,
            DirectPursuit
        }

        private State _currentState = State.PathFinding;


        public void Update(Unit unit, float deltaTime, IBoardContext context)
        {
            if(deltaTime <= 0f) return; // No time has passed, no update needed.
            if(!unit.IsAlive)
            {
                unit.MoveSpeed *= 0.95f; // Needs deltaTime Scaled, but good enough for now, and has no game effect yet.
                return;
            }

            Unit? targetUnit = TargetUnitValid(unit.TargetUnit) ? unit.TargetUnit : null;

            // State changes first.
            switch(_currentState)
            {
                case State.Idle:
                    // Does nothing. May include auto aggression or something later
                    break;
                case State.PathFinding:
                {
                    // No current path. Even if we have a path, this means it is invalid.
                    Pathfind(unit, context); // Pathfind. This also changes the state appropriately. 
                    break;
                }
                case State.PathFollowing:
                {
                    if(ValidPath())
                    {
                        // If we can see the next waypoint, skip to it.
                        if(context.Map?.HasLineOfSight(context.Map.ToGrid(unit.Position), Path![CurrentWaypoint + 1]) == true)
                        {
                            CurrentWaypoint++;
                        }
                        _TargetPosition = context.Map!.FromGrid(Path![CurrentWaypoint]);
                    }
                    else
                    {
                        _currentState = State.PathFinding; // Lost path somehow, try to repath.
                    }
                    break;
                }
                case State.DirectPursuit:
                {
                    if(targetUnit != null)
                    {
                        AssertUtil.NotNull(context.Map);
                        if(context.Map.HasLineOfSight(unit.Position, targetUnit.Position, unit.Radius))
                        {
                            // Continue direct pursuit.
                            _TargetPosition = targetUnit.Position;
                        }
                        else
                        {
                            // Lost target. Repath.
                            _currentState = State.PathFinding;
                        }
                    }
                    else
                    {
                        _currentState = State.Idle; // No valid target, give up.
                    }
                    break;
                }
                default:
                {
                    Debug.Assert(false, "Unknown AI State");
                    break;
                }
            }

            

            //    Unit? target = unit.Target;

            //if(target != null)
            //{
            //    float distSqrt = unit.DistanceSquaredTo(target);
            //    _TargetPosition = target.Position;
            //
            //    if(unit.Weapon != null)
            //    {
            //        if(distSqrt <= unit.Weapon.AttackRangeSqr
            //            && context.Map?.HasLineOfSight(context.Map.ToGrid(unit.Position), context.Map.ToGrid(target.Position)) == true)
            //        {
            //            unit.Weapon.TryShootEntity(unit, target, context);
            //            // Might have to mess with control states, but the unit's movement automatically slows during cooldown, so for now, no need.
            //            // And, the weapon likely controls that slowdown later.
            //        }
            //        // Might query the Weapon if I should be even trying to move or not. Later.
            //
            //    }
            //
            //
            //    // Pathfinding logic
            //    if(context.PathFinder != null && context.Map != null)
            //    {
            //        // Whole thing breaks if you don't have a Pathfinder and a map.
            //
            //        if(_currentState == State.Idle)
            //        {
            //            Pathfind(unit, context);
            //            AssertUtil.Assert(ValidPath(), "Pathfinding failed to find a valid path.");
            //            // would set to idle in such a failure state, maybe.
            //        }
            //        else if(_currentState == State.FollowingPath)
            //        { 
            //            // Was pathfinding. Look Ahead, and validate.
            //            if(ValidPath())
            //            {
            //                // If we have a valid path, check if we can shortcut to the next waypoint.
            //                if(context.Map.HasLineOfSight(context.Map.ToGrid(unit.Position), Path![CurrentWaypoint+1]))
            //                {
            //                    // We can see the next waypoint directly, so it is our new current waypoint.
            //                    CurrentWaypoint++;
            //                    // This is still pathfinding.
            //                }
            //            }
            //            else
            //            {
            //                // Lost our path somehow. Repath.
            //                Pathfind(unit, context);
            //                AssertUtil.Assert(ValidPath(), "Pathfinding failed to find a valid path.");
            //            }
            //        }
            //
            //        // Recalculate path if no valid path exists OR
            //        // if the target has moved significantly since the last path calculation
            //        if(!ValidPath() || Vector2.DistanceSquared(_TargetPosition, _lastTargetPosition) > TargetRecalculateThresholdSqr)
            //        {
            //            Pathfind(unit, context);
            //            // Assert that a valid path was found. This might be expected to fail in some situations, but for now, assume it should always work.
            //            // (e.g., by stopping movement or finding a new target).
            //            AssertUtil.Assert(ValidPath(), "Pathfinding failed to find a valid path.");
            //            // would set to idle in such a failure state, maybe.
            //        }
            //
            //        if(ValidPath())
            //        {
            //            // Shortcutting: Find the furthest visible waypoint
            //            int currentWaypointIndex = CurrentWaypoint;
            //            for(int i = CurrentWaypoint; i < Path!.Length; i++)
            //            {
            //                // Check if the unit has line of sight to this waypoint
            //                // and if it's a valid tile to move towards (e.g., not blocked)
            //                if(context.Map.HasLineOfSight(context.Map.ToGrid(unit.Position), Path[i]))
            //                {
            //                    if (currentWaypointIndex != i)
            //                        _currentState = State.ApproachingTarget; // We can see this waypoint, so we are beelining towards it.
            //                    currentWaypointIndex = i; // This waypoint is visible, try to go to it
            //                }
            //                else
            //                {
            //                    // If we lose LOS to the current waypoint, stop looking further
            //                    break;
            //                }
            //            }
            //
            //            _TargetPosition = context.Map.FromGrid(Path[currentWaypointIndex]);
            //
            //            distSqrt = Vector2.DistanceSquared(unit.Position, _TargetPosition);
            //            // If the unit is close enough to the current waypoint, advance to the next one
            //            if(distSqrt <= 4f && CurrentWaypoint < Path.Length - 1) // 4f is 2 units squared. Might be too long, or I may want alternate method.
            //            {
            //                CurrentWaypoint++; // Move to the next waypoint
            //                _TargetPosition = context.Map.FromGrid(Path[CurrentWaypoint]); // Update targetPos to the new waypoint
            //                _currentState = State.FollowingPath; // We are pathfinding here, not shortcutting anymore if we were previously.
            //            }
            //            // If we are at the last waypoint and close enough, clear the path
            //            else if(distSqrt <= 4f && CurrentWaypoint == Path.Length - 1)
            //            {
            //                Path = null; // Reached the end of the path
            //                CurrentWaypoint = 0;
            //                unit.TargetPoint = unit.Position; // Stop moving
            //                _currentState = State.Idle; // Set state to idle, I guess.
            //                                            // Pretty sure this will never happen given current game assumptions.
            //                return; // Path completed, no further movement needed this frame
            //            }
            //        }
            //        else
            //        {
            //            Pathfind(unit, context); // Try to pathfind again.
            //
            //            AssertUtil.Assert(ValidPath(), "Pathfinding failed to find a valid path.");
            //            // Strictly speaking, I don't think this is a good way to do this.
            //            // Failure may be valid in some cases.
            //
            //            AssertUtil.NotNull(context.Map);
            //            _TargetPosition = context.Map.FromGrid(Path![CurrentWaypoint]);
            //            _lastTargetPosition = _TargetPosition; // Update the last known target position.
            //
            //        }
            //    }
            //
            //    // Calculate steering force towards the target position (waypoint or direct target)
            //    Vector2 steeringTarget = GetPartialSteeringTarget(unit, _TargetPosition, context);
            //
            //    //Vector2 steeringTarget = Vector2.Normalize(targetPos - unit.Position);
            //
            //    // Anti-bunching: Add separation force from nearby friendly units
            //    // This assumes IBoardContext can provide nearby units. You might need to implement
            //    // a method like GetNearbyUnits(unit.Position, separationRadius) in IBoardContext.
            //    // For demonstration, let's assume context.GetAllUnits() exists and we filter.
            //    Vector2 separationForce = Vector2.Zero;
            //    
            //    IEnumerable<Unit> entities = context.Entities.Entities
            //        .OfType<Unit>()
            //        .Where(u => u.IsAlive && u.Team.IsFriendlyTo(unit.Team))
            //        .ToList(); // Get all friendly units
            //
            //    separationForce = GetSeparationSteering(unit, entities, context);
            //    separationForce *= 1f;
            //
            //    // Combine steering forces (path following + separation).
            //    // You might want to weight these forces based on priority.
            //    Vector2 combinedSteering = Vector2.Normalize(steeringTarget + separationForce);
            //
            //
            //    if(combinedSteering != Vector2.Zero) // Ensure we have a direction to move
            //    {
            //        Vector2 dir = combinedSteering * unit.MoveSpeed * deltaTime; // Multiply by deltaTime for frame-rate independence
            //        unit.TargetPoint = unit.Position + dir;
            //    }
            //    else
            //    {
            //        unit.TargetPoint = unit.Position; // No movement needed.
            //    }
            //}
            //else
            //{
            //    // No target, Keep doing what you're doing.
            //    // Well, ideally, follow the path properly, but That should really be a method call.
            //}
        }

        private static bool TargetUnitValid(Unit? targetUnit)
        {
            return targetUnit != null && targetUnit.IsAlive;
        }

        private void Pathfind(Unit unit, IBoardContext context)
        {
            Pathfind(unit, _TargetPosition, context);
            _currentState = ValidPath() ? State.PathFollowing : State.Idle;
            _lastPathfind = context.CurrentTime;
            _lastTargetPosition = _TargetPosition; // Update the last known target position.
        }

        /// <summary>
        /// Checks if a valid path currently exists and if the next waypoint is within bounds.
        /// </summary>
        /// <returns>True if a valid path exists, false otherwise.</returns>
        public bool ValidPath()
        {
            return Path != null && Path.Length > 0 && CurrentWaypoint < Path.Length;
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
                CurrentWaypoint = 0;

                if(Path[0] == startTile && Path.Length > 1)
                {
                    CurrentWaypoint = 1; // Skip first waypoint if it's the current tile.
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

            if(map.HasLineOfSight(unit.Position, targetPos,unit.Radius))
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
            AssertUtil.Assert(!bestDir.IsNaN());
            return bestDir;
        }
        /// <summary>
        /// Calculates a steering force to push the unit away from nearby friendly units (anti-bunching).
        /// </summary>
        /// <param name="unit">The current unit.</param>
        /// <param name="allUnits">A collection of all units in the game (or at least nearby units).</param>
        /// <param name="context">The board context.</param>
        /// <returns>A vector representing the separation force.</returns>
        private static Vector2 GetSeparationSteering(Unit unit, IEnumerable<Unit> allUnits, IBoardContext context)
        {
            Vector2 separationForce = Vector2.Zero;
            float separationRadius = unit.Radius * 4;
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

                string stateText = $"AI: ";
                if (_currentState == State.Idle)
                    stateText += "Idle";
                else if (_currentState == State.FollowingPath)
                    stateText += "Pathfinding";
                else if (_currentState == State.ApproachingTarget)
                    stateText += "Shortcutting";
                else
                    stateText += "Unknown";

                if (!TimeUtils.HasElapsed(context.CurrentTime, _lastPathfind, 0.1f))
                    stateText += "\n (Path refreshed)";

                using var font = new Font("Arial", 8);
                using var brush = new SolidBrush(Color.White);
                g.DrawString(stateText, font, brush, unit.Position.X + unit.Radius, unit.Position.Y - unit.Radius);
            }

            if(ValidPath())
            {
                AssertUtil.NotNull(Path); // ValidPath already does this, but compiler insists.
                float tileSize = map.TileSize / 4;
                int x = CurrentWaypoint;
                Vector2 currentPointWorld = (x == CurrentWaypoint) ? unit.Position : map.FromGrid(Path[x-1]);
                Vector2 nextPointWorld = map.FromGrid(Path[x]);
               
                
                if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerNextWaypoint))
                {
                    // Next waypoint and line connecting it.
                    g.DrawLine(Pens.Yellow, currentPointWorld.ToPoint(), nextPointWorld.ToPoint());
                    Vector2 currentTargetWaypointWorld = map.FromGrid(Path![CurrentWaypoint]);
                    tileSize = map.TileSize / 3; // Make it slightly larger
                    g.FillRectangle(Brushes.Red, currentTargetWaypointWorld.X - tileSize / 2, currentTargetWaypointWorld.Y - tileSize / 2, tileSize, tileSize);

                }

                if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerFullPath))
                {
                    // Draw lines between path points and highlight waypoints
                    for(x = CurrentWaypoint; x < Path.Length; x++) // Start drawing from the current waypoint
                    {
                        currentPointWorld = (x == CurrentWaypoint) ? unit.Position : map.FromGrid(Path[x - 1]);
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
                    //if (unit.TargetPoint != null)
                    map.DrawDebugLineOfSight(g, unit.Position, _TargetPosition, unit.Radius);
                }
            }
        }
    }
}
