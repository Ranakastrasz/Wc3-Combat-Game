using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util.UnitConversion
{
    public class GameSpace
    {
        //public const float PHYSICS_SCALE = 1/64f;
        //public const float GRID_SCALE = 1/32f;

        enum Space
        {
            World,
            Physics,
            Grid
        }


        class SpaceConverterFloat: UnitConverter<Space, float>
        {
            static SpaceConverterFloat()
            {
                BaseUnit = Space.World;

                // Correct Physics <-> World conversions
                RegisterConversion(Space.Physics,
                    w => w * 64f,
                    p => p / 64f);

                // Correct Grid <-> World conversions
                RegisterConversion(Space.Grid,
                    w => w * 32f,
                    g => g / 32f);
            }
        }

        /// <summary>
        /// Represents a distance in WorldSpace.
        /// </summary>
        public readonly struct WorldFloat
        {
            public float Value { get; }

            public WorldFloat(float value)
            {
                Value = value;
            }

            // Explicit conversion from WorldFloat to PhysicsFloat
            public static explicit operator PhysicsFloat(WorldFloat w)
            {
                var converter = new SpaceConverterFloat();
                var physicsValue = converter.Convert(w.Value, Space.World, Space.Physics);
                return new PhysicsFloat(physicsValue);
            }

            public override string ToString() => $"{Value} W";
        }
        /// <summary>
        /// Represents a distance in PhysicsSpace.
        /// </summary>
        public readonly struct PhysicsFloat
        {
            public float Value { get; }
            public PhysicsFloat(float value)
            {
                Value = value;
            }
            // Explicit conversion from PhysicsFloat to WorldFloat
            public static explicit operator WorldFloat(PhysicsFloat p)
            {
                var converter = new SpaceConverterFloat();
                var worldValue = converter.Convert(p.Value, Space.Physics, Space.World);
                return new WorldFloat(worldValue);
            }
            public override string ToString() => $"{Value} P";
        }

        /// <summary>
        /// represents a distance in GridSpace.
        /// </summary>
        public readonly struct GridFloat
        {
            public float Value { get; }
            public GridFloat(float value)
            {
                Value = value;
            }
            // Explicit conversion from GridFloat to WorldFloat
            public static explicit operator WorldFloat(GridFloat g)
            {
                var converter = new SpaceConverterFloat();
                var worldValue = converter.Convert(g.Value, Space.Grid, Space.World);
                return new WorldFloat(worldValue);
            }
            public override string ToString() => $"{Value} G";
        }

    }
}