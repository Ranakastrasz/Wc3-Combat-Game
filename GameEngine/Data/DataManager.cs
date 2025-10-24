using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            if(_unitData.ContainsKey(prototype.ID))
                throw new ArgumentException($"A UnitData with the ID '{prototype.ID}' is already registered.");
            _unitData[prototype.ID] = prototype;
        }
        public static void RegisterAbility(AbilityData prototype)
        {
            if(_abilityData.ContainsKey(prototype.ID))
                throw new ArgumentException($"An AbilityData with the ID '{prototype.ID}' is already registered.");
            _abilityData[prototype.ID] = prototype;
        }
        public static void RegisterProjectile(ProjectileData prototype)
        {
            if(_projectileData.ContainsKey(prototype.ID))
                throw new ArgumentException($"A ProjectileData with the ID '{prototype.ID}' is already registered.");
            _projectileData[prototype.ID] = prototype;
        }
        public static void RegisterGameplayAction(IGameplayAction action)
        {
            if(_actionData.ContainsKey(action.ID))
                throw new ArgumentException($"A GameplayAction with the ID '{action.ID}' is already registered.");
            _actionData[action.ID] = action;
        }

        internal static bool TryGetUnit(string v, [NotNullWhen(true)] out UnitData? unit)
        {
            return _unitData.TryGetValue(v, out unit);
        }

        internal static bool TryGetAbility(string v, [NotNullWhen(true)] out AbilityData? ability)
        {
            return _abilityData.TryGetValue(v, out ability);
        }

        internal static bool TryGetProjectile(string v, [NotNullWhen(true)] out ProjectileData? projectile)
        {
            return _projectileData.TryGetValue(v, out projectile); // Errors out for no fucking reason.
            //if(_projectileData.TryGetValue(v, out ProjectileData data))
            //{
            //    projectile = data;
            //    return true;
            //}
            //projectile = null;
            //return false;
        }
        internal static bool TryGetGameplayAction(string v, [NotNullWhen(true)] out IGameplayAction? action)
        {
            return _actionData.TryGetValue(v, out action);
        }

        public static StringBuilder Draw()
        {
            StringBuilder oString = new StringBuilder();
            oString.AppendLine("=== Registered Units ===");
            foreach(var unit in _unitData.Values)
            {
                oString.AppendLine($"- {unit.ID}: {unit.Name}");
            }
            oString.AppendLine("=== Registered Abilities ===");
            foreach(var ability in _abilityData.Values)
            {
                oString.AppendLine($"- {ability.ID}: {ability.Name}");
            }
            oString.AppendLine("=== Registered Projectiles ===");
            foreach(var projectile in _projectileData.Values)
            {
                oString.AppendLine($"- {projectile.ID}: Projectile");
            }
            oString.AppendLine("=== Registered Actions ===");
            foreach(var action in _actionData.Values)
            {
                oString.AppendLine($"- {action.ID}: Action");
            }
            return oString;
        }

        public static void Flush()
        {
            _unitData.Clear();
            _abilityData.Clear();
            _projectileData.Clear();
            _actionData.Clear();
        }
    }
}