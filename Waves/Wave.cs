using Wc3_Combat_Game.Entities.Units.Prototypes;

namespace Wc3_Combat_Game.Waves
{
    public struct Wave
    {
        public UnitPrototype Unit { get; }
        public int CountToSpawn { get; }

        public Wave(UnitPrototype unit, int toSpawn)
        {
            Unit = unit;
            CountToSpawn = toSpawn;
        }

    }
}




