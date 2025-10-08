using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Point = Microsoft.Xna.Framework.Point;

namespace Wc3_Combat_Game.Entities.Units.Controllers
{
    public class UnitMovementController
    {
        public Point[]? Path = null;
        public int CurrentWaypoint = 0;

        //// Store the target's position when the path was last calculated
        //private Vector2 _lastTargetPosition = Vector2.Zero;
        //// Threshold for target movement to trigger path recalculation
        //private const float TargetRecalculateThresholdSqr = 32*32; // If target moves more than 32 units.
        //private const float SeparationDistanceSqr = 5f*5f; // Minimum distance to maintain from other friendly units.
        //
        //
        //private Vector2 _TargetMovePosition;
        //private float _lastPathfind = 0f;
        //private const float PathRecalculationInterval = 0.5f; // Minimum time between path recalculations.
        //
        //private float _lastLostPath = 0f;
        //private const float RecoveryDelay = 1f; // Time to wait after losing path before trying to recover.
        //private float _lastPartialSteering = 0;
        //
        //private const float WaypointTolerance = 2f*2f; // Distance to final waypoint to consider "arrived"

    }
}