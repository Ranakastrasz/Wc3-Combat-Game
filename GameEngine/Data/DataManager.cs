using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.GameEngine.Data.Data;


namespace Wc3_Combat_Game.GameEngine.Data
{
    public class DataManager
    {

        private static Dictionary<string, UnitData> _unitData { get; } = new();
        private static Dictionary<string, AbilityData> _abilityData { get; } = new();
        private static Dictionary<string, ProjectileData> _projectileData { get; } = new();
        private static Dictionary<string, IGameplayAction> _actionData { get; } = new();

        private DataManager() { }

        public static void RegisterUnit(UnitData prototype)
        {
            if (_unitData.ContainsKey(prototype.ID))
                throw new ArgumentException($"A UnitData with the ID '{prototype.ID}' is already registered.");
            _unitData[prototype.ID] = prototype;
        }
        public static void RegisterAbility(AbilityData prototype)
        {
            if (_abilityData.ContainsKey(prototype.ID))
                throw new ArgumentException($"An AbilityData with the ID '{prototype.ID}' is already registered.");
            _abilityData[prototype.ID] = prototype;
        }
        public static void RegisterProjectile(ProjectileData prototype)
        {
            if (_projectileData.ContainsKey(prototype.ID))
                throw new ArgumentException($"A ProjectileData with the ID '{prototype.ID}' is already registered.");
            _projectileData[prototype.ID] = prototype;
        }
        public static void RegisterGameplayAction(IGameplayAction action)
        {
            if (_actionData.ContainsKey(action.ID))
                throw new ArgumentException($"A GameplayAction with the ID '{action.ID}' is already registered.");
            _actionData[action.ID] = action;
        }

        internal static UnitData TryGetUnit(string v)
        {
            if (_unitData.TryGetValue(v, out var prototype))
                return prototype;
            throw new KeyNotFoundException($"No UnitData found with ID '{v}'.");
        }

        internal static bool TryGetAbility(string v, out AbilityData? ability)

        {
            if (_abilityData.TryGetValue(v, out var prototype))
            {
                ability = prototype;
                return true;
            }
            ability = null;
            return false;
        }

        internal static ProjectileData TryGetProjectile(string v)
        {
            if (_projectileData.TryGetValue(v, out var prototype))
                return prototype;
            throw new KeyNotFoundException($"No ProjectileData found with ID '{v}'.");
        }
        internal static IGameplayAction TryGetGameplayAction(string v)
        {
            if (_actionData.TryGetValue(v, out var action))
                return action;
            throw new KeyNotFoundException($"No GameplayAction found with ID '{v}'.");
        }

    }
}