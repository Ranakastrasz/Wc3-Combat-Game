using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Interface.Controllers
{
    class IBasicAIController : IUnitController
    {
        public void Update(Unit unit, float deltaTime, BoardContext context)
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

                Vector2 dir = GeometryUtils.NormalizeAndScale(target.Position - unit.Position, unit.Speed);
                unit.Move(dir);
            }

        }
    }
}
