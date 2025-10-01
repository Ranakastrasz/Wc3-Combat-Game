using System.Collections.Immutable;

using Wc3_Combat_Game.Entities.Units.Prototypes;

namespace Wc3_Combat_Game.Waves
{
    public record Wave
    {
        public ImmutableList<Tuple<UnitPrototype, int>> UnitAndCount { get; }

        public UnitPrototype Unit => UnitAndCount[0].Item1;
        public int CountToSpawn => UnitAndCount[0].Item2;

        private Wave(ImmutableList<Tuple<UnitPrototype, int>> unitAndCount)
        {
            UnitAndCount = unitAndCount;
        }
        public Wave(UnitPrototype unit, int toSpawn)
        {
            UnitAndCount = ImmutableList.Create(Tuple.Create(unit, toSpawn));
        }
        public Wave AddUnit(UnitPrototype unit, int count)
        {
            var newUnitAndCount = UnitAndCount.Add(Tuple.Create(unit, count));
            return new Wave(newUnitAndCount);
        }
    }
}




