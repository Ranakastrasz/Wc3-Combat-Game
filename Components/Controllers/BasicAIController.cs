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
        private const float TargetRecalculateThresholdSqr = 32*32; // If target moves more than 32 units.
        private const float SeparationDistanceSqr = 20f*20f; // Minimum distance to maintain from other friendly units.


        private Vector2 _TargetMovePosition;
        private float _lastPathfind = 0f;
        private const float PathRecalculationInterval = 0.5f; // Minimum time between path recalculations.

        private float _lastLostPath = 0f;
        private const float RecoveryDelay = 1f; // Time to wait after losing path before trying to recover.
        private float _lastPartialSteering = 0; 

        private const float WaypointTolerance = 2f*2f; // Distance to final waypoint to consider "arrived"


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
            AssertUtil.NotNull(context.Map);
            _TargetMovePosition = unit.Position;

            if(!TargetUnitValid(unit.TargetUnit) || unit.GetTargetPosition() is not Vector2 targetPosition)
            {
                _currentState = State.Idle;
                return;
            }

            var canSeeTarget = CanSeeTarget(unit, context);

            // State changes first.
            switch(_currentState)
            {
                case State.Idle:
                    // Does nothing. May include auto aggression or something later
                    break;
                case State.PathFinding:
                {
                    // No current path. Even if we have a path, this means it is invalid.
                    if(TryPathfind(unit, context))
                    {
                        _currentState = State.PathFollowing;
                    }
                    else
                    {
                        _currentState = State.Idle; // Failed to find a path, give up.
                    }
                    break;
                }
                case State.PathFollowing:
                {
                    if(canSeeTarget)
                    {
                        _currentState = State.DirectPursuit;
                        break; // Will handle in next state.
                    }
                    if(_lastTargetPosition != targetPosition 
                        && Vector2.DistanceSquared(_lastTargetPosition, targetPosition) > TargetRecalculateThresholdSqr
                        && TimeUtils.HasElapsed(context.CurrentTime, _lastPathfind, PathRecalculationInterval)) // Limit to 1 per 2 seconds.
                    {
                        // Target has moved significantly since last path calculation. Repath.
                        _currentState = State.PathFinding;
                        break;
                    }
                    else if(ValidPath())
                    {
                        // If we can see the next waypoint, skip to it.
                        // Slight issue. We need to validate waypoint here.
                        if(CurrentWaypoint < Path!.Length - 1) // Ensure there is a next waypoint
                        {
                            var nextWaypoint = Path![CurrentWaypoint + 1];
                            if(unit.HasClearPathTo(context.Map.FromGrid(nextWaypoint),context))
                            {
                                CurrentWaypoint++;
                            }
                        }
                        else
                        { 
                            // No further waypoints available. Check for when we arrive, then re-pathfind.
                            float distSqr = Vector2.DistanceSquared(unit.Position, context.Map!.FromGrid(Path![CurrentWaypoint]));
                            if (distSqr <= WaypointTolerance)
                            {
                                // Reached the end of the path. Since we didn't spot the target,
                                // but still have one, build a new path.
                                _currentState = State.PathFinding;
                            }
                        }

                        // Failsafe: check LOS to current waypoint. If blocked, re-path.
                        if(!unit.HasClearPathTo(context.Map.FromGrid(Path[CurrentWaypoint]), context))
                        {
                            if(CurrentWaypoint != 0)
                            {
                                // Check previous waypoint, to see if recovery is possible.
                                Point lastWaypoint = Path![CurrentWaypoint-1];
                                if(unit.HasClearPathTo(context.Map.FromGrid(lastWaypoint), context))
                                {
                                    CurrentWaypoint--;
                                }
                                else
                                {
                                    if(_lastLostPath == 0) // first time losing path
                                    {
                                        _lastLostPath = context.CurrentTime;
                                        // fallback steering now
                                        GetPartialSteeringTarget(unit, _TargetMovePosition, context);
                                        _lastPartialSteering = context.CurrentTime;
                                    }
                                    else if(TimeUtils.HasElapsed(context.CurrentTime, _lastLostPath, RecoveryDelay))
                                    {
                                        _currentState = State.PathFinding; // delay expired, try to repath
                                        _lastLostPath = 0; // reset timer
                                    }
                                    else
                                    {
                                        // Still waiting for delay, fallback steering
                                        GetPartialSteeringTarget(unit, _TargetMovePosition, context);
                                        _lastPartialSteering = context.CurrentTime;
                                    }
                                }

                            }
                            else
                            {
                                // No previous waypoint. Give up and repath.
                                _currentState = State.PathFinding;
                            }
                            break;
                        }

                        _TargetMovePosition = context.Map!.FromGrid(Path![CurrentWaypoint]);
                    }
                    else
                    {
                        _currentState = State.PathFinding; // Lost path somehow, try to repath.
                    }
                    break;
                }
                case State.DirectPursuit:
                {
                    if(canSeeTarget)
                    {
                        // Continue direct pursuit.
                        _TargetMovePosition = targetPosition;
                    }
                    else
                    {
                        // Lost target. Repath.
                        _currentState = State.PathFinding;
                    }
                    break;
                }
                default:
                {
                    Debug.Assert(false, "Unknown AI State");
                    break;
                }
            }

            // Action phase: Attack if possible, then move towards target position.
            // Ideally, you delegate this to the unit, I.e. Unit.TryShootEntity/Point.
            if(unit.Weapon != null && canSeeTarget)
            {
                float distSqr = unit.DistanceSquaredTo(targetPosition);
                if(distSqr <= unit.Weapon.AttackRangeSqr)
                    if(unit.TargetUnit != null)
                        unit.Weapon.TryShootEntity(unit, unit.TargetUnit, context);
                    else
                        unit.Weapon.TryShootPoint(unit, targetPosition, context);
            }

            if(_TargetMovePosition != unit.Position)
            {
                Vector2 steeringTarget = Vector2.Normalize(_TargetMovePosition - unit.Position);

                // Get friendly units for separation
                IEnumerable<Unit> entities = context.Entities.Entities
                    .OfType<Unit>()
                    .Where(u => u.IsAlive && u.Team.IsFriendlyTo(unit.Team))
                    .ToList();

                Vector2 separationForce = GetSeparationSteering(unit, entities, context);

                // Decompose separation
                float forwardDot = Vector2.Dot(steeringTarget, separationForce);
                Vector2 forwardComponent = steeringTarget * MathF.Min(forwardDot, 0f); // Clamp to ≤ 0
                Vector2 lateralComponent = separationForce - forwardComponent;

                // Tunable weights
                float forwardOppositionWeight = 1.0f;  // How much to slow when blocked
                float lateralSeparationWeight = 1.5f;  // How much to sidestep

                // Apply weighted components
                Vector2 adjustedSeparation =
                    forwardComponent * forwardOppositionWeight +
                    lateralComponent * lateralSeparationWeight;

                // Combine with target movement
                Vector2 combined = steeringTarget + adjustedSeparation;

                // Normalize if needed
                if(combined.LengthSquared() < 0.01f)
                    combined = steeringTarget; // Don't stop completely unless really necessary
                else
                    combined = Vector2.Normalize(combined);

                // Move the unit
                Vector2 dir = combined * unit.MoveSpeed * deltaTime;
                unit.TargetPoint = unit.Position + dir;
            }
            else
            {
                unit.TargetPoint = unit.Position; // No movement needed.
            }
        }

        private bool CanSeeTarget(Unit unit, IBoardContext context)
        {
            Vector2? targetPosition = unit.GetTargetPosition();
            return targetPosition != null && unit.HasClearPathTo(targetPosition.Value, context);
        }

        private static bool TargetUnitValid(Unit? targetUnit)
        {
            return targetUnit != null && targetUnit.IsAlive;
        }

        /// <summary>
        /// Checks if a valid path currently exists and if the next waypoint is within bounds.
        /// </summary>
        /// <returns>True if a valid path exists, false otherwise.</returns>
        public bool ValidPath()
        {
            return Path != null && Path.Length > 0 && CurrentWaypoint < Path.Length;
        }

        private bool TryPathfind(Unit unit, IBoardContext context)
        {
            Vector2? targetPos = unit.GetTargetPosition();
            if (targetPos == null) return false; // No target to pathfind to.
            Pathfind(unit, targetPos.Value, context);
            _lastPathfind = context.CurrentTime;
            _lastTargetPosition = targetPos.Value; // Update the last known target position.
            return ValidPath();
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
        /// Calculates a steering vector towards a target position
        /// </summary>
        /// <param name="myPos">The current position of the unit.</param>
        /// <param name="targetPos">The target world position.</param>
        /// <param name="context">The board context providing map information.</param>
        /// <returns>A normalized steering vector, or Vector2.Zero if already at target.</returns>
        public static Vector2 GetPartialSteeringTarget(Unit unit, Vector2 targetPos, IBoardContext context)
        {
            var map = context.Map!;
            if(unit.Position == targetPos)
                return Vector2.Zero;

            Point myTile = map.ToGrid(unit.Position);
            Point targetTile = map.ToGrid(targetPos);

            if(map.HasLineOfSight(unit.Position, targetPos, unit.Radius))
                return Vector2.Normalize(targetPos - unit.Position);

            Vector2 bestDir = Vector2.Zero;
            float bestDist = float.MaxValue;

            foreach(Point neighbor in map.GetAdjacentTiles(myTile))
            {
                if(!map[neighbor].IsWalkable)
                    continue;

                float dist = Vector2.DistanceSquared(new Vector2(neighbor.X, neighbor.Y), new Vector2(targetTile.X, targetTile.Y));
                if(dist < bestDist)
                {
                    bestDist = dist;
                    bestDir = Vector2.Normalize(map.FromGrid(neighbor) - unit.Position);
                }
            }
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
                switch(_currentState)
                {
                    case State.Idle:
                        stateText += "Idle";
                        break;
                    case State.PathFinding:
                        stateText += "PathFinding";
                        break;
                    case State.PathFollowing:
                        stateText += $"PathFollowing ({(ValidPath() ? Path!.Length - CurrentWaypoint : 0)} waypoints left)";
                        break;
                    case State.DirectPursuit:
                        stateText += "DirectPursuit";
                        break;
                    default:
                        stateText += "Unknown";
                        break;

                }


                if (!TimeUtils.HasElapsed(context.CurrentTime, _lastPathfind, 0.1f))
                    stateText += "\n (Path refreshed)";
                if (!TimeUtils.HasElapsed(context.CurrentTime, _lastPartialSteering, 0.1f))
                    stateText += "\n (Partial steering active)";
                using var font = new Font("Arial", 8);
                using var brush = new SolidBrush(Color.White);
                g.DrawString(stateText, font, brush, unit.Position.X + unit.Radius, unit.Position.Y - unit.Radius);
            }

            if(ValidPath())
            {
                AssertUtil.NotNull(Path); // ValidPath already does this, but compiler insists.
                float tileSize = map.TileSize * 0.1f;
                int x = CurrentWaypoint;
                Vector2 currentPointWorld = (x == CurrentWaypoint) ? unit.Position : map.FromGrid(Path[x-1]);
                Vector2 nextPointWorld = map.FromGrid(Path[x]);
               
                
                if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerNextWaypoint))
                {
                    if(_currentState == State.PathFollowing)
                    {
                        // Next waypoint and line connecting it.
                        g.DrawLine(Pens.Yellow, currentPointWorld.ToPoint(), nextPointWorld.ToPoint());
                        Vector2 currentTargetWaypointWorld = map.FromGrid(Path![CurrentWaypoint]);
                        tileSize = map.TileSize * 0.15f; // Make it slightly larger
                        g.FillRectangle(Brushes.Red, currentTargetWaypointWorld.X - tileSize / 2, currentTargetWaypointWorld.Y - tileSize / 2, tileSize, tileSize);
                    }
                    else if (_currentState == State.DirectPursuit)
                    {
                        // Direct pursuit target position
                        g.DrawLine(Pens.AliceBlue, unit.Position.ToPoint(), _TargetMovePosition.ToPoint());

                    }
                }

                if(context.DebugSettings.Get(DebugSetting.DrawEnemyControllerFullPath))
                {
                    int indexOffset = unit.Index % 32; // Keep it bounded (adjust modulus as needed)
                    float angle = (float)(indexOffset * (Math.PI * 2 / 32)); // Spread evenly around a circle
                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 16f; // 4 pixels radius
                    //Vector2 offset = new Vector2(indexOffset/32f -0.5f, indexOffset/32f -0.5f) * map.TileSize; // Center of tile
                    // Draw lines between path points and highlight waypoints
                    for(x = CurrentWaypoint; x < Path.Length; x++) // Start drawing from the current waypoint
                    {
                        currentPointWorld = (x == CurrentWaypoint) ? unit.Position : map.FromGrid(Path[x - 1]);
                        nextPointWorld = map.FromGrid(Path[x]);
                            
                        Point from = currentPointWorld.ToPoint();
                        Point to = (nextPointWorld + offset).ToPoint();
                        if(x != CurrentWaypoint)
                        {
                            // Always center on the unit itself.
                            from = (currentPointWorld + offset).ToPoint();
                        }
                        // Draw the line segment
                        g.DrawLine(Pens.Yellow, from, to);

                        // Draw a rectangle at each waypoint
                        g.FillRectangle(Brushes.Yellow, to.X - tileSize / 2, to.Y - tileSize / 2, tileSize, tileSize);
                    }
                }
                if (context.DebugSettings.Get(DebugSetting.DrawEnemyControllerLOS))
                {
                    // Draw line of sight Check to next waypoint, via asking the map to draw it.
                    if (_currentState == State.PathFollowing)
                        map.DrawDebugLineOfSight(g, unit.Position, context.Map.FromGrid(Path[CurrentWaypoint]), unit.Radius);
                    else if (_currentState == State.DirectPursuit)
                        map.DrawDebugLineOfSight(g, unit.Position, unit.GetTargetPosition().Value, unit.Radius);
                }
            }
        }
    }
}
