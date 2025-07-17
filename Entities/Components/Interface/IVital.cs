namespace Wc3_Combat_Game.Entities.Components.Interface
{
    internal interface IVital
    {
        public float? Health { get; protected set; }
        public float? Shield { get; protected set; }
        public float? Mana { get; protected set; }

        public bool TrySpend(string type, float amount);
        public void TakeDamage(float dmg);

    }
}
