using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util.UnitConversion
{
    public class GameSpace
    {
        public const float PHYSICS_SCALE = 64f;
        public const float GRID_SCALE = 32f;

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
                    w => w * PHYSICS_SCALE,
                    p => p / PHYSICS_SCALE);

                // Correct Grid <-> World conversions
                RegisterConversion(Space.Grid,
                    w => w * GRID_SCALE,
                    g => g / GRID_SCALE);
            }
        }
        private static readonly SpaceConverterFloat _floatConverter = new SpaceConverterFloat();


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
                var physicsValue = _floatConverter.Convert(w.Value, Space.World, Space.Physics);
                return new PhysicsFloat(physicsValue);
            }

            public static explicit operator GridFloat(WorldFloat w)
            {
                var gridValue = _floatConverter.Convert(w.Value, Space.World, Space.Grid);
                return new GridFloat(gridValue);
            }
            public static explicit operator float(WorldFloat w) => w.Value;

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
                var worldValue = _floatConverter.Convert(p.Value, Space.Physics, Space.World);
                return new WorldFloat(worldValue);
            }
            public static explicit operator GridFloat(PhysicsFloat p)
            {
                var gridValue = _floatConverter.Convert(p.Value, Space.Physics, Space.Grid);
                return new GridFloat(gridValue);
            }
            public static explicit operator float(PhysicsFloat p) => p.Value;
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
                var worldValue = _floatConverter.Convert(g.Value, Space.Grid, Space.World);
                return new WorldFloat(worldValue);
            }
            public override string ToString() => $"{Value} G";
        }


        class SpaceConverterVector2 : UnitConverter<Space, System.Numerics.Vector2>
        {
            static SpaceConverterVector2()
            {
                BaseUnit = Space.World;
                // Correct Physics <-> World conversions
                RegisterConversion(Space.Physics,
                    w => w * PHYSICS_SCALE,
                    p => p / PHYSICS_SCALE);
                // Correct Grid <-> World conversions
                RegisterConversion(Space.Grid,
                    w => w * GRID_SCALE,
                    g => g / GRID_SCALE);
            }
        }
        private static readonly SpaceConverterVector2 _vector2Converter = new SpaceConverterVector2();

        public readonly struct WorldVector2
        {
            public Vector2 Value { get; }
            public WorldVector2(Vector2 value)
            {
                Value = value;
            }
            // Explicit conversion from WorldVector2 to PhysicsVector2
            public static explicit operator PhysicsVector2(WorldVector2 w)
            {
                var physicsValue = _vector2Converter.Convert(w.Value, Space.World, Space.Physics);
                return new PhysicsVector2(physicsValue);
            }
            public static explicit operator GridVector2(WorldVector2 w)
            {
                var gridValue = _vector2Converter.Convert(w.Value, Space.World, Space.Grid);
                return new GridVector2(gridValue);
            }
            public static explicit operator Vector2(WorldVector2 w) => w.Value;

            public override string ToString() => $"{Value} W";
        }

        public readonly struct PhysicsVector2
        {
            public Vector2 Value { get; }
            public PhysicsVector2(Vector2 value)
            {
                Value = value;
            }
            // Explicit conversion from PhysicsVector2 to WorldVector2
            public static explicit operator WorldVector2(PhysicsVector2 p)
            {
                var worldValue = _vector2Converter.Convert(p.Value, Space.Physics, Space.World);
                return new WorldVector2(worldValue);
            }
            public static explicit operator GridVector2(PhysicsVector2 p)
            {
                var gridValue = _vector2Converter.Convert(p.Value, Space.Physics, Space.Grid);
                return new GridVector2(gridValue);
            }
            public static explicit operator Vector2(PhysicsVector2 p) => p.Value;
            public override string ToString() => $"{Value} P";
        }

        public readonly struct GridVector2
        {
            public Vector2 Value { get; }
            public GridVector2(Vector2 value)
            {
                Value = value;
            }
            // Explicit conversion from GridVector2 to WorldVector2
            public static explicit operator WorldVector2(GridVector2 g)
            {
                var worldValue = _vector2Converter.Convert(g.Value, Space.Grid, Space.World);
                return new WorldVector2(worldValue);
            }
            public static explicit operator PhysicsVector2(GridVector2 g)
            {
                var physicsValue = _vector2Converter.Convert(g.Value, Space.Grid, Space.Physics);
                return new PhysicsVector2(physicsValue);
            }
            public static explicit operator Vector2(GridVector2 g) => g.Value;
            public override string ToString() => $"{Value} G";
        }
    }
}