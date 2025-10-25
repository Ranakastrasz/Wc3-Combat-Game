using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.GameEngine.Data.Data;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.GameEngine.Data.Factories
{
    public static class GameActionFactory
    {
        public static DamageAction CreateDamageAction(float damage)
        {
            return new DamageAction($"damage_action_{damage}", damage);
        }

        public static AoeDamageAction CreateAoeDamageAction(float damage, float minDamage, WorldLength aoeRadius, bool centerOnTarget = false)
        {
            return new AoeDamageAction($"aoe_damage_action_{damage}_{minDamage}_{aoeRadius}", damage, minDamage, aoeRadius, centerOnTarget);
        }
        
        public static AoeAction CreateAoeAction(IGameplayAction innerAction, WorldLength aoeRadius, bool centerOnTarget = false)
        {
            return new AoeAction($"aoe_action_{aoeRadius}_{innerAction.ID}", aoeRadius, innerAction, centerOnTarget);
        }

        public static BuffAction CreateBuffAction(IBuffable.BuffType buffType, float factor, float duration)
        {
            return new BuffAction($"buff_action_{buffType}_{factor}_{duration}", buffType, factor, duration);
        }

        public static ProjectileAction CreateProjectileAction(string baseId, ProjectileData projectileData)
        {
            return new ProjectileAction($"{baseId}_{projectileData.ID}", projectileData);
        }


        // Dunno if I really need these, but eh, for now seems sensible.
        public static BuffAction CreateRecoilDebuffAction(float recoilFactor, float recoilDuration)
        {
            return new BuffAction($"recoil_debuff_action_{recoilFactor}_{recoilDuration}", IBuffable.BuffType.Slow, recoilFactor, recoilDuration);
        }

        public static BuffAction CreateSnareDebuffAction(float snareFactor, float snareDuration)
        {
            return new BuffAction($"snare_debuff_action_{snareFactor}_{snareDuration}", IBuffable.BuffType.Slow, snareFactor, snareDuration);
        }

        public static BuffAction CreateSprintBuffAction(float speedFactor, float sprintDuration)
        {
            return new BuffAction($"sprint_buff_action_{speedFactor}_{sprintDuration}", IBuffable.BuffType.Speed, speedFactor, sprintDuration);
        }

    }
}