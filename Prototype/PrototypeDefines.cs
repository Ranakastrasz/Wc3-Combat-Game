using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototypes;

namespace Wc3_Combat_Game.Prototypes
{
    internal class PrototypeDefines
    {
        public static readonly float PLAYER_COOLDOWN = 0.35f;

        public const float PROJECTILE_SPEED = 1200f;
        public static readonly SizeF PROJECTILE_SIZE = new(10, 10);
        public static readonly Brush PROJECTILE_COLOR = Brushes.Blue;

        public const float PROJECTILE_LIFESPAN = 0.25f;
        public const float PLAYER_DAMAGE = 10f;

        public static ProjectilePrototype PLAYER_PROJECTILE = new ProjectilePrototype
            (10f,
            1200f,
            0.25f,
            new EffectDamage(10f),
            Brushes.Blue);


        public static ProjectilePrototype ENEMY_PROJECTILE = new ProjectilePrototype
            (
                10f,
                100f,
                1f,
                new EffectDamage(10f),
                Brushes.Cyan
            );



    }
}
