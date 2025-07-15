namespace Wc3_Combat_Game.Components.Entitys
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
