using System.Collections.Immutable;

using Wc3_Combat_Game.GameEngine.Data;
using Wc3_Combat_Game.GameEngine.Data.Data;

namespace Wc3_Combat_Game.GameEngine.Waves
{
    public record Wave
    {
        public ImmutableList<Tuple<string, int>> UnitAndCount { get; }

        public UnitData Unit => DataManager.TryGetUnit(UnitAndCount[0].Item1, out var unit) ? unit : throw new InvalidOperationException("Unit not found.");
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




