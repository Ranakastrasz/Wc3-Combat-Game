using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.GameEngine.Data.Data;

namespace Wc3_Combat_Game.GameEngine.Data.Factories
{
    public static class ProjectileFactory
    {
        public static ProjectileData CreateBasicProjectile (float radius, float speed, float lifespan, IGameplayAction? impactAction, int polygonCount, Color color)
        {
            return new ProjectileData($"basic_projectile_{radius}_{speed}_{lifespan}_{(impactAction != null ? impactAction.ID : "n/a")}_{polygonCount}_{color}", radius, speed, lifespan, impactAction, polygonCount, color);
        }

    }
}