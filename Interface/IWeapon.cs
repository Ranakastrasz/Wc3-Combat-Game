using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game
{
    internal interface IWeapon
    {
        float GetAttackRange(); // For AI mostly.
        float GetAttackRangeSqr() => GetAttackRange()*GetAttackRange(); // For AI mostly.

        bool TryShoot(Unit unit, Vector2 target, float currentTime);
        float GetCooldown();             // For display
        float GetTimeSinceLastShot(float currentTime); // For display

    }

}
