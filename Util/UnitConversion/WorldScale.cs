global using WorldPoint = Wc3_Combat_Game.Util.UnitConversion.WorldVector;
global using PhysicsPoint = Wc3_Combat_Game.Util.UnitConversion.PhysicsVector;
global using GridPoint = Wc3_Combat_Game.Util.UnitConversion.GridVector;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Wc3_Combat_Game.Util.UnitConversion
{

    public readonly struct WorldLength
    {
        public const float WorldToPhysicsFactor = 32f;
        public const float WorldToGridFactor = 64f;

        public float Value { get; }

        public WorldLength(float value) => Value = value;

        public static WorldLength FromWorld(float value) => new WorldLength(value);
        public float PhysicsValue => Value / WorldToPhysicsFactor;
        public float GridValue => Value / WorldToGridFactor;

        public static WorldLength operator +(WorldLength a, WorldLength b) => new WorldLength(a.Value + b.Value);
        public static WorldLength operator -(WorldLength a, WorldLength b) => new WorldLength(a.Value - b.Value);
        public static WorldLength operator *(WorldLength a, float factor) => new WorldLength(a.Value * factor);
        public static WorldLength operator *(float factor, WorldLength a) => a * factor;
        public static WorldLength operator /(WorldLength a, float factor) => new WorldLength(a.Value / factor);
        public static float operator /(WorldLength a, WorldLength b) => a.Value / b.Value;

        public static bool operator ==(WorldLength a, WorldLength b) => a.Value == b.Value;
        public static bool operator !=(WorldLength a, WorldLength b) => a.Value != b.Value;
        public static bool operator > (WorldLength a, WorldLength b) => a.Value > b.Value;
        public static bool operator >=(WorldLength a, WorldLength b) => a.Value >= b.Value;
        public static bool operator < (WorldLength a, WorldLength b) => a.Value < b.Value;
        public static bool operator <=(WorldLength a, WorldLength b) => a.Value <= b.Value;
        public override bool Equals(object? obj) => obj is WorldLength other && this == other;
        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"{Value:F2} World";
    }

    // Physics scale: World / 32
    public readonly struct PhysicsLength
    {
        public float Value { get; }

        public PhysicsLength(float value) => Value = value;

        // Implicit conversion from WorldLength to PhysicsLength (World / 32)
        public static implicit operator PhysicsLength(WorldLength w) => new PhysicsLength(w.Value / WorldLength.WorldToPhysicsFactor);

        // Explicit conversion from PhysicsLength to WorldLength (Physics * 32)
        public static explicit operator WorldLength(PhysicsLength p) => new WorldLength(p.Value * WorldLength.WorldToPhysicsFactor);

        public static PhysicsLength FromPhysics(float value) => new PhysicsLength(value);
        public float WorldValue => Value * WorldLength.WorldToPhysicsFactor;
        public float GridValue => Value * (WorldLength.WorldToPhysicsFactor / WorldLength.WorldToGridFactor);

        public static PhysicsLength operator +(PhysicsLength a, PhysicsLength b) => new PhysicsLength(a.Value + b.Value);
        public static PhysicsLength operator -(PhysicsLength a, PhysicsLength b) => new PhysicsLength(a.Value - b.Value);
        public static PhysicsLength operator *(PhysicsLength a, float factor) => new PhysicsLength(a.Value * factor);
        public static PhysicsLength operator *(float factor, PhysicsLength a) => a * factor;
        public static PhysicsLength operator /(PhysicsLength a, float factor) => new PhysicsLength(a.Value / factor);
        public static float operator /(PhysicsLength a, PhysicsLength b) => a.Value / b.Value;

        public static bool operator ==(PhysicsLength a, PhysicsLength b) => a.Value == b.Value;
        public static bool operator !=(PhysicsLength a, PhysicsLength b) => a.Value != b.Value;
        public static bool operator > (PhysicsLength a, PhysicsLength b) => a.Value > b.Value;
        public static bool operator >=(PhysicsLength a, PhysicsLength b) => a.Value >= b.Value;
        public static bool operator < (PhysicsLength a, PhysicsLength b) => a.Value < b.Value;
        public static bool operator <=(PhysicsLength a, PhysicsLength b) => a.Value <= b.Value;
        public override bool Equals(object? obj) => obj is PhysicsLength other && this == other;
        public override int GetHashCode() => Value.GetHashCode();


        public override string ToString() => $"{Value:F2} Physics";
    }

    // Grid scale: World / 64
    public readonly struct GridLength
    {
        public float Value { get; }
        public GridLength(float value) => Value = value;

        // Implicit conversion from WorldLength to GridLength (World / 64)
        public static implicit operator GridLength(WorldLength w) => new GridLength(w.Value / WorldLength.WorldToGridFactor);

        // Explicit conversion from GridLength to WorldLength (Grid * 64)
        public static explicit operator WorldLength(GridLength g) => new WorldLength(g.Value * WorldLength.WorldToGridFactor);

        public static GridLength FromGrid(float value) => new GridLength(value);

        public float WorldValue => Value * WorldLength.WorldToGridFactor;
        public float PhysicsValue => Value * (WorldLength.WorldToGridFactor / WorldLength.WorldToPhysicsFactor);

        public static GridLength operator +(GridLength a, GridLength b) => new GridLength(a.Value + b.Value);
        public static GridLength operator -(GridLength a, GridLength b) => new GridLength(a.Value - b.Value);
        public static GridLength operator *(GridLength a, float factor) => new GridLength(a.Value * factor);
        public static GridLength operator *(float factor, GridLength a) => a * factor;
        public static GridLength operator /(GridLength a, float factor) => new GridLength(a.Value / factor);
        public static float operator /(GridLength a, GridLength b) => a.Value / b.Value;

        public static bool operator ==(GridLength a, GridLength b) => a.Value == b.Value;
        public static bool operator !=(GridLength a, GridLength b) => a.Value != b.Value;
        public static bool operator >(GridLength a, GridLength b) => a.Value > b.Value;
        public static bool operator >=(GridLength a, GridLength b) => a.Value >= b.Value;
        public static bool operator <(GridLength a, GridLength b) => a.Value < b.Value;
        public static bool operator <=(GridLength a, GridLength b) => a.Value <= b.Value;
        public override bool Equals(object? obj) => obj is GridLength other && this == other;
        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"{Value:F2} Grid";
    }

    //-------------------------------------------------------------------------
    // VECTOR2 STRUCTS
    //-------------------------------------------------------------------------

    // World scale: Base scale
    public readonly struct WorldVector
    {
        public Vector2 Value { get; }
        public readonly float X => Value.X;
        public readonly float Y => Value.Y;

        public WorldVector(Vector2 value)
        {
            Value = value;
        }
        public WorldVector(float x, float y)
        {
            Value = new Vector2(x, y);
        }

        // Simple arithmetic operator overloads
        public static WorldVector operator +(WorldVector a, WorldVector b) => new WorldVector(a.Value + b.Value);
        public static WorldVector operator -(WorldVector a, WorldVector b) => new WorldVector(a.Value - b.Value);
        public static WorldVector operator *(WorldVector a, float s) => new WorldVector(a.Value * s);
        public static WorldVector operator *(float s, WorldVector a) => a * s;
        public static WorldVector operator /(WorldVector a, float s) => new WorldVector(a.Value / s);
        public static float operator /(WorldVector a, WorldVector b) => a.Value.Length() / b.Value.Length();

        // Comparison operators based on vector length

        public static bool operator ==(WorldVector a, WorldVector b) => a.Value == b.Value;
        public static bool operator !=(WorldVector a, WorldVector b) => a.Value != b.Value;
        public static bool operator > (WorldVector a, WorldVector b) => a.Value.LengthSquared() > b.Value.LengthSquared();
        public static bool operator >=(WorldVector a, WorldVector b) => a.Value.LengthSquared() >= b.Value.LengthSquared();
        public static bool operator < (WorldVector a, WorldVector b) => a.Value.LengthSquared() < b.Value.LengthSquared();
        public static bool operator <=(WorldVector a, WorldVector b) => a.Value.LengthSquared() <= b.Value.LengthSquared();

        public override bool Equals(object? obj) => obj is WorldVector other && this == other;
        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"({X:F2}, {Y:F2}) World";

    }

    // Physics scale: World / 32
    public readonly struct PhysicsVector
    {
        public Vector2 Value { get; }
        public readonly float X => Value.X;
        public readonly float Y => Value.Y;

        public PhysicsVector(Vector2 value)
        {
            Value = value;
        }
        public PhysicsVector(float x, float y)
        {
            Value = new Vector2(x, y);
        }

        // Implicit conversion from WorldVector2 to PhysicsVector2 (World / 32)
        public static implicit operator PhysicsVector(WorldVector w) =>
            new PhysicsVector(
                w.X / WorldLength.WorldToPhysicsFactor,
                w.Y / WorldLength.WorldToPhysicsFactor);

        // Explicit conversion from PhysicsVector2 to WorldVector2 (Physics * 32)
        public static explicit operator WorldVector(PhysicsVector p) =>
            new WorldVector(
                p.X * WorldLength.WorldToPhysicsFactor,
                p.Y * WorldLength.WorldToPhysicsFactor);

        public static PhysicsVector operator +(PhysicsVector a, PhysicsVector b) => new PhysicsVector(a.Value + b.Value);
        public static PhysicsVector operator -(PhysicsVector a, PhysicsVector b) => new PhysicsVector(a.Value - b.Value);
        public static PhysicsVector operator *(PhysicsVector a, float s) => new PhysicsVector(a.Value * s);
        public static PhysicsVector operator *(float s, PhysicsVector a) => a * s;
        public static WorldVector operator /(PhysicsVector a, float s) => new WorldVector(a.Value / s);
        public static float operator /(PhysicsVector a, PhysicsVector b) => a.Value.Length() / b.Value.Length();


        // Comparison operators based on vector length

        public static bool operator ==(PhysicsVector a, PhysicsVector b) => a.Value == b.Value;
        public static bool operator !=(PhysicsVector a, PhysicsVector b) => a.Value != b.Value;
        public static bool operator > (PhysicsVector a, PhysicsVector b) => a.Value.LengthSquared() > b.Value.LengthSquared();
        public static bool operator >=(PhysicsVector a, PhysicsVector b) => a.Value.LengthSquared() >= b.Value.LengthSquared();
        public static bool operator < (PhysicsVector a, PhysicsVector b) => a.Value.LengthSquared() < b.Value.LengthSquared();
        public static bool operator <=(PhysicsVector a, PhysicsVector b) => a.Value.LengthSquared() <= b.Value.LengthSquared();

        public override bool Equals(object? obj) => obj is PhysicsVector other && this == other;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"({X:F5}, {Y:F5}) Physics";
    }

    // Grid scale: World / 64
    public readonly struct GridVector
    {
        public Vector2 Value { get; }
        public readonly float X => Value.X;
        public readonly float Y => Value.Y;
        public GridVector(Vector2 value)
        {
            Value = value;
        }
        public GridVector(float x, float y)
        {
            Value = new Vector2(x, y);
        }

        // Implicit conversion from WorldVector2 to GridVector2 (World / 64)
        public static implicit operator GridVector(WorldVector w) =>
            new GridVector(
                w.X / WorldLength.WorldToGridFactor,
                w.Y / WorldLength.WorldToGridFactor);

        // Explicit conversion from GridVector2 to WorldVector2 (Grid * 64)
        public static explicit operator WorldVector(GridVector g) =>
            new WorldVector(
                g.X * WorldLength.WorldToGridFactor,
                g.Y * WorldLength.WorldToGridFactor);
        public static GridVector operator +(GridVector a, GridVector b) => new GridVector(a.Value + b.Value);
        public static GridVector operator -(GridVector a, GridVector b) => new GridVector(a.Value - b.Value);
        public static GridVector operator *(GridVector a, float s) => new GridVector(a.Value * s);
        public static GridVector operator *(float s, GridVector a) => a * s;
        public static WorldVector operator /(GridVector a, float s) => new WorldVector(a.Value / s);
        public static float operator /(GridVector a, GridVector b) => a.Value.Length() / b.Value.Length();

        // Comparison operators based on vector length

        public static bool operator ==(GridVector a, GridVector b) => a.Value == b.Value;
        public static bool operator !=(GridVector a, GridVector b) => a.Value != b.Value;
        public static bool operator > (GridVector a, GridVector b) => a.Value.LengthSquared() > b.Value.LengthSquared();
        public static bool operator >=(GridVector a, GridVector b) => a.Value.LengthSquared() >= b.Value.LengthSquared();
        public static bool operator < (GridVector a, GridVector b) => a.Value.LengthSquared() < b.Value.LengthSquared();
        public static bool operator <=(GridVector a, GridVector b) => a.Value.LengthSquared() <= b.Value.LengthSquared();
        public override bool Equals(object? obj) => obj is GridVector other && this == other;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"({X:F5}, {Y:F5}) Grid";
    }

    //-------------------------------------------------------------------------
    // EXTENSIONS AND DEMONSTRATION
    //-------------------------------------------------------------------------
    public static class ScaleExtensions
    {
        // For Lengths (No change)
        public static WorldLength World(this float value) => WorldLength.FromWorld(value);
        public static PhysicsLength Physics(this float value) => PhysicsLength.FromPhysics(value);
        public static GridLength Grid(this float value) => GridLength.FromGrid(value);

        // For Vector2 (Fluent construction)
        public static WorldVector World(this Vector2 value) => new WorldVector(value);
        public static WorldVector WorldVector(this (float x, float y) values) => new WorldVector(values.x, values.y);
        public static PhysicsVector PhysicsVector(this Vector2 value) => new PhysicsVector(value);
        public static PhysicsVector PhysicsVector(this (float x, float y) values) => new PhysicsVector(values.x, values.y);
        public static GridVector GridVector(this Vector2 value) => new GridVector(value);
        public static GridVector GridVector(this (float x, float y) values) => new GridVector(values.x, values.y);
    }

    public class ScaleDemonstration
    {
        public static void Run()
        {
            // Fluent Construction (like your 5.Meters() example)
            WorldLength baseDistance = 5f.World();
            PhysicsLength physDistance = 10f.Physics();
            GridLength gridDistance = 256f.Grid();

            System.Console.WriteLine($"Base: {baseDistance}");   // 5.00 World
            System.Console.WriteLine($"Phys: {physDistance}");   // 10.00 Physics
            System.Console.WriteLine($"Grid: {gridDistance}");   // 256.00 Grid
            System.Console.WriteLine("---------------------");

            // --- Conversion (Implicit from World, Explicit to World) ---

            // Implicit: World -> Physics (5 World * 32 = 160 Physics)
            PhysicsLength convertedPhysics = baseDistance;
            System.Console.WriteLine($"5 World in Physics: {convertedPhysics}"); // 160.00 Physics

            // Explicit: Physics -> World (10 Physics / 32 = 0.3125 World)
            WorldLength convertedWorld = (WorldLength)physDistance;
            System.Console.WriteLine($"10 Physics in World: {convertedWorld}");  // 0.31 World

            // Getting the value in other scales (via explicit conversion)
            GridLength physInGrid = (GridLength)((WorldLength)physDistance);
            System.Console.WriteLine($"10 Physics in Grid: {physInGrid}"); // 20.00 Grid (0.3125 * 64)

            System.Console.WriteLine("---------------------");

            // --- Operator Overloads ---

            // Addition (requires matching types)
            WorldLength addedWorld = baseDistance + convertedWorld;
            System.Console.WriteLine($"Sum: {addedWorld}"); // 5.31 World

            // You must explicitly convert before adding:
            // GridLength gridSum = gridDistance + physInGrid; // OK
            WorldLength sumOfAllWorld = baseDistance + (WorldLength)physDistance + (WorldLength)gridDistance;
            System.Console.WriteLine($"Sum of All in World: {sumOfAllWorld}"); // 5.00 + 0.31 + 4.00 = 9.31 World

            // Multiplication/Division with scalar
            WorldLength multiplied = baseDistance * 3;
            System.Console.WriteLine($"Multiplied: {multiplied}"); // 15.00 World

            // Vector2 usage example
            WorldVector wVec1 = (2f, 4f).WorldVector();
            WorldLength wLength = 10f.World();

            // Vector arithmetic
            WorldVector wVec2 = wVec1 * 2;
            System.Console.WriteLine($"Vector: {wVec2}"); // (4.00, 8.00) World

            System.Console.WriteLine("--- Projectile Scale Conversions ---");

            // Define values in the World (base) unit
            WorldLength projectileSizeWorld = 5f.World();
            WorldLength projectileSpeedWorld = 450f.World();
            WorldLength projectileAoeWorld = 32f.World();

            // Conversion to Physics (Implicit conversion from World)
            PhysicsLength sizePhysics = projectileSizeWorld;
            PhysicsLength speedPhysics = projectileSpeedWorld;
            PhysicsLength aoePhysics = projectileAoeWorld;

            // Conversion to Grid (Implicit conversion from World)
            GridLength sizeGrid = projectileSizeWorld;
            GridLength speedGrid = projectileSpeedWorld;
            GridLength aoeGrid = projectileAoeWorld;

            // Output Results
            System.Console.WriteLine($"Projectile Size:");
            System.Console.WriteLine($"  World: {projectileSizeWorld.Value:F5}"); // 5.00000 World
            System.Console.WriteLine($"  Physics: {sizePhysics.Value:F5}");       // 0.15625 Physics (5/32)
            System.Console.WriteLine($"  Grid: {sizeGrid.Value:F5}");            // 0.07813 Grid (5/64)

            System.Console.WriteLine($"\nProjectile Speed:");
            System.Console.WriteLine($"  World: {projectileSpeedWorld.Value:F5}");// 450.00000 World
            System.Console.WriteLine($"  Physics: {speedPhysics.Value:F5}");      // 14.06250 Physics (450/32)
            System.Console.WriteLine($"  Grid: {speedGrid.Value:F5}");           // 7.03125 Grid (450/64)

            System.Console.WriteLine($"\nProjectile AOE:");
            System.Console.WriteLine($"  World: {projectileAoeWorld.Value:F5}"); // 32.00000 World
            System.Console.WriteLine($"  Physics: {aoePhysics.Value:F5}");        // 1.00000 Physics (32/32)
            System.Console.WriteLine($"  Grid: {aoeGrid.Value:F5}");             // 0.50000 Grid (32/64)

        }
    }
}