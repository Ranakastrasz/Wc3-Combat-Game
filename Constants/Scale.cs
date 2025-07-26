using System.Numerics; // Or UnityEngine.Vector2 if you're in Unity

namespace Wc3_Combat_Game.Constants
{
    // Define marker interfaces for each scale type
    public interface IGameSpace { }     // Logical world coordinates (internal default)
    public interface IPhysicsSpace { }  // Aether
    public interface IRenderSpace { }   // For drawing, affected by camera zoom
    public interface ITileSpace { }     // Integer tile grid


    public readonly struct Vector2Quantity<T>
    {
        public readonly Vector2 Value { get; }
        public Vector2Quantity(Vector2 value) => Value = value;

        public static implicit operator Vector2(Vector2Quantity<T> m) => m.Value;
        public static explicit operator Vector2Quantity<T>(Vector2 v) => new(v);

        public override string ToString() => $"[{typeof(T).Name}] {Value}";
    }

    public readonly struct FloatQuantity<T>
    {
        public readonly float Value { get; }
        public FloatQuantity(float value) => Value = value;

        public static implicit operator float(FloatQuantity<T> m) => m.Value;
        public static explicit operator FloatQuantity<T>(float v) => new(v);

        public override string ToString() => $"[{typeof(T).Name}] {Value}";
    }

    public static class UnitConversions
    {
        public const float GameUnitsPerTile = 32f;
        public const float GameUnitsPerPhysics = 64f;

        // Game ↔ Physics
        public static Vector2Quantity<IPhysicsSpace> ToPhysics(this Vector2Quantity<IGameSpace> g)
            => new(g.Value / GameUnitsPerPhysics);
        public static Vector2Quantity<IGameSpace> ToGame(this Vector2Quantity<IPhysicsSpace> p)
            => new(p.Value * GameUnitsPerPhysics);
        public static FloatQuantity<IPhysicsSpace> ToPhysics(this FloatQuantity<IGameSpace> g)
            => new(g.Value / GameUnitsPerPhysics);
        public static FloatQuantity<IGameSpace> ToGame(this FloatQuantity<IPhysicsSpace> p)
            => new(p.Value * GameUnitsPerPhysics);

        // Game ↔ Tile
        public static Vector2Quantity<ITileSpace> ToTile(this Vector2Quantity<IGameSpace> g)
            => new(g.Value / GameUnitsPerTile);
        public static Vector2Quantity<IGameSpace> ToGame(this Vector2Quantity<ITileSpace> t)
            => new(t.Value * GameUnitsPerTile);
        public static FloatQuantity<ITileSpace> ToTile(this FloatQuantity<IGameSpace> g)
            => new(g.Value / GameUnitsPerTile);
        public static FloatQuantity<IGameSpace> ToGame(this FloatQuantity<ITileSpace> t)
            => new(t.Value * GameUnitsPerTile);

        // Physics ↔ Tile
        public static Vector2Quantity<ITileSpace> ToTile(this Vector2Quantity<IPhysicsSpace> p)
            => new(p.Value * GameUnitsPerPhysics / GameUnitsPerTile);
        public static Vector2Quantity<IPhysicsSpace> ToPhysics(this Vector2Quantity<ITileSpace> t)
            => new(t.Value * GameUnitsPerTile / GameUnitsPerPhysics);
        public static FloatQuantity<ITileSpace> ToTile(this FloatQuantity<IPhysicsSpace> p)
            => new(p.Value * GameUnitsPerPhysics / GameUnitsPerTile);
        public static FloatQuantity<IPhysicsSpace> ToPhysics(this FloatQuantity<ITileSpace> t)
            => new(t.Value * GameUnitsPerTile / GameUnitsPerPhysics);

        public static Vector2Quantity<IRenderSpace> ToRender(this Vector2Quantity<IGameSpace> g)
            => new(g.Value);
        public static Vector2Quantity<IGameSpace> ToGame(this Vector2Quantity<IRenderSpace> r)
            => new(r.Value);
        public static FloatQuantity<IRenderSpace> ToRender(this FloatQuantity<IGameSpace> g)
            => new(g.Value);
        public static FloatQuantity<IGameSpace> ToGame(this FloatQuantity<IRenderSpace> r)
            => new(r.Value);
    }
}