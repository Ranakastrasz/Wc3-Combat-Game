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
        public static PrototypeManager Instance { get; } = new PrototypeManager();

        public Dictionary<string, UnitPrototype> UnitPrototypes { get; } = new();
        public Dictionary<string, AbilityPrototype> AbilityPrototypes { get; } = new();
        public Dictionary<string, ProjectilePrototype> ProjectilePrototypes { get; } = new();
        public Dictionary<string, IGameplayAction> GameplayActions { get; } = new();

        private PrototypeManager() { }

        public void RegisterUnit(UnitPrototype prototype)
        {
            if (UnitPrototypes.ContainsKey(prototype.Id))
                throw new ArgumentException($"A UnitPrototype with the ID '{prototype.Id}' is already registered.");
            UnitPrototypes[prototype.Id] = prototype;
        }
        //public void RegisterAbility(AbilityPrototype prototype)
        //{
        //    if (AbilityPrototypes.ContainsKey(prototype.Id))
        //        throw new ArgumentException($"An AbilityPrototype with the ID '{prototype.Id}' is already registered.");
        //    AbilityPrototypes[prototype.Id] = prototype;
        //}
        //public void RegisterProjectile(ProjectilePrototype prototype)
        //{
        //    if (ProjectilePrototypes.ContainsKey(prototype.Id))
        //        throw new ArgumentException($"A ProjectilePrototype with the ID '{prototype.Id}' is already registered.");
        //    ProjectilePrototypes[prototype.Id] = prototype;
        //}
        //public void RegisterGameplayAction(IGameplayAction action)
        //{
        //    if (GameplayActions.ContainsKey(action.Id))
        //        throw new ArgumentException($"A GameplayAction with the ID '{action.Id}' is already registered.");
        //    GameplayActions[action.Id] = action;
        //}

    }
}