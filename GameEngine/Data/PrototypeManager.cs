using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Entities.Projectiles.Prototypes;
using Wc3_Combat_Game.Entities.Units.Abilities;
using Wc3_Combat_Game.Entities.Units.Prototypes;
using Wc3_Combat_Game.GameEngine.Actions.Interface;

/*
 
var mgr = PrototypeManager.Instance;
mgr.RegisterAbility(new AbilityPrototype("Dash", ...));
mgr.RegisterAbility(new AbilityPrototype("Heal", ...));
// Unit prototypes can be built now, even though they reference the ability IDs
mgr.RegisterUnit(new UnitPrototype("BasicGrunt", 100, "Dash")); 
 */

namespace Wc3_Combat_Game.GameEngine.Data
{
    public class PrototypeManager
    {
        //public static PrototypeManager Instance { get; } = new PrototypeManager();

        private static Dictionary<string, UnitPrototype> UnitPrototypes { get; } = new();
        private static Dictionary<string, AbilityPrototype> AbilityPrototypes { get; } = new();
        private static Dictionary<string, ProjectilePrototype> ProjectilePrototypes { get; } = new();
        private static Dictionary<string, IGameplayAction> GameplayActions { get; } = new();

        private PrototypeManager() { }

        public static void RegisterUnit(UnitPrototype prototype)
        {
            if (UnitPrototypes.ContainsKey(prototype.ID))
                throw new ArgumentException($"A UnitPrototype with the ID '{prototype.ID}' is already registered.");
            UnitPrototypes[prototype.ID] = prototype;
        }
        public static void RegisterAbility(AbilityPrototype prototype)
        {
            if (AbilityPrototypes.ContainsKey(prototype.ID))
                throw new ArgumentException($"An AbilityPrototype with the ID '{prototype.ID}' is already registered.");
            AbilityPrototypes[prototype.ID] = prototype;
        }
        //public void RegisterProjectile(ProjectilePrototype prototype)
        //{
        //    if(ProjectilePrototypes.ContainsKey(prototype.ID))
        //        throw new ArgumentException($"A ProjectilePrototype with the ID '{prototype.Id}' is already registered.");
        //    ProjectilePrototypes[prototype.ID] = prototype;
        //}

        internal static UnitPrototype GetUnit(string v)
        {
            if (UnitPrototypes.TryGetValue(v, out var prototype))
                return prototype;
            throw new KeyNotFoundException($"No UnitPrototype found with ID '{v}'.");
        }

        internal static AbilityPrototype GetAbility(string v)
        {
            if (AbilityPrototypes.TryGetValue(v, out var prototype))
                return prototype;
            throw new KeyNotFoundException($"No AbilityPrototype found with ID '{v}'.");
        }

        internal static ProjectilePrototype GetProjectile(string v)
        {
            if (ProjectilePrototypes.TryGetValue(v, out var prototype))
                return prototype;
            throw new KeyNotFoundException($"No ProjectilePrototype found with ID '{v}'.");
        }
        internal static IGameplayAction GetGameplayAction(string v)
        {
            if (GameplayActions.TryGetValue(v, out var action))
                return action;
            throw new KeyNotFoundException($"No GameplayAction found with ID '{v}'.");
        }
        //public void RegisterGameplayAction(IGameplayAction action)
        //{
        //    if (GameplayActions.ContainsKey(action.Id))
        //        throw new ArgumentException($"A GameplayAction with the ID '{action.Id}' is already registered.");
        //    GameplayActions[action.Id] = action;
        //}

    }
}