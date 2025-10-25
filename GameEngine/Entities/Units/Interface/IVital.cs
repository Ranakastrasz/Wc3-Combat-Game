namespace Wc3_Combat_Game.Entities.Units.Interface
{
    /// <summary>
    /// This is to be attached to a unit to handle the highly fluid vital stats, such as Health, shield, and Mana.
    /// 
    /// </summary>
    public interface IVital
    {
        public float? Health { get; protected set; }
        public float? Shield { get; protected set; }
        public float? Mana { get; protected set; }

        public bool TrySpend(string type, float amount);
        public void TakeDamage(float dmg);

    }
}
