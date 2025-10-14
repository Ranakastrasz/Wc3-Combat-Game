using System.Collections.Immutable;

using Wc3_Combat_Game.Entities.Units.Prototypes;
using Wc3_Combat_Game.GameEngine.Data;

namespace Wc3_Combat_Game.GameEngine.Waves
{
    public record Wave
    {
        public ImmutableList<Tuple<string, int>> UnitAndCount { get; }

        public UnitPrototype Unit => PrototypeManager.TryGetUnit(UnitAndCount[0].Item1);
        public int CountToSpawn => UnitAndCount[0].Item2;

        private Wave(ImmutableList<Tuple<string, int>> unitAndCount)
        {
            UnitAndCount = unitAndCount;
        }
        public Wave(string unit, int toSpawn)
        {
            UnitAndCount = ImmutableList.Create(Tuple.Create(unit, toSpawn));
        }
        public Wave AddUnit(string unit, int count)
        {
            var newUnitAndCount = UnitAndCount.Add(Tuple.Create(unit, count));
            return new Wave(newUnitAndCount);
        }
    }
}




