namespace Wc3_Combat_Game.IO
{
    public enum DebugSetting
    {
        DrawEntityMovementVector,
        DrawEntityCollisionBox,
        DrawEnemyControllerState,
        DrawEnemyControllerFullPath,
        DrawEnemyControllerNextWaypoint,
        DrawEnemyControllerLOS,
        //DrawTowerRange, // no towers in game.
        //DrawTowerTargetLine,
        //DrawTowerCooldown,
        //DrawProjectileHomingLine, // Could be interesting
        //DrawProjectileTargetLine, // Not even sure what this would do.
        ShowEntityBounds,
        ShowEntityIDs,
        ShowEntityCount,
        ShowFPS,
        ShowGameTime,
        ShowCameraInfo,
        ShowInputInfo
    }

    public static class DebugSettingExtensions
    {
        private static readonly Dictionary<DebugSetting, string> _displayNames = new()
        {
            [DebugSetting.DrawEntityMovementVector] = "Draw.Entity.MovementVector",
            [DebugSetting.DrawEntityCollisionBox] = "Draw.Entity.CollisionBox",
            [DebugSetting.DrawEnemyControllerState] = "Draw.Entity.Controller.State",
            [DebugSetting.DrawEnemyControllerFullPath] = "Draw.Entity.Controller.FullPath",
            [DebugSetting.DrawEnemyControllerNextWaypoint] = "Draw.Entity.Controller.NextWaypoint",
            [DebugSetting.DrawEnemyControllerLOS] = "Draw.Entity.Controller.LOS",
            //[DebugSetting.DrawTowerRange] = "Draw.Tower.Range",
            //[DebugSetting.DrawTowerTargetLine] = "Draw.Tower.TargetLine",
            //[DebugSetting.DrawTowerCooldown] = "Draw.Tower.Cooldown",
            //[DebugSetting.DrawProjectileHomingLine] = "Draw.Projectile.HomingLine",
            //[DebugSetting.DrawProjectileTargetLine] = "Draw.Projectile.TargetLine",
            [DebugSetting.ShowEntityBounds] = "Show.EntityBounds",
            [DebugSetting.ShowEntityIDs] = "Show.EntityIDs",
            [DebugSetting.ShowEntityCount] = "Show.EntityCount",
            [DebugSetting.ShowFPS] = "Show.FPS",
            [DebugSetting.ShowGameTime] = "Show.GameTime",
            [DebugSetting.ShowCameraInfo] = "Show.CameraInfo",
            [DebugSetting.ShowInputInfo] = "Show.InputInfo"
        };

        public static string ToDisplayString(this DebugSetting setting)
        {
            return _displayNames.TryGetValue(setting, out var name) ? name : setting.ToString();
        }

        public static DebugSetting FromDisplayString(string displayString)
        {
            foreach(var kvp in _displayNames)
            {
                if(kvp.Value == displayString)
                    return kvp.Key;
            }
            throw new KeyNotFoundException($"Debug setting display string '{displayString}' not found.");
        }

        public static string[] AllDisplayStrings()
        {
            return _displayNames.Values.ToArray();
        }
    }

    public class DebugSettings
    {
        private readonly Dictionary<DebugSetting, bool> settings = new()
        {
            [DebugSetting.DrawEntityMovementVector] = false,        // Implemented
            [DebugSetting.DrawEntityCollisionBox] = false,          // Implemented
            [DebugSetting.DrawEnemyControllerState] = false,       
            [DebugSetting.DrawEnemyControllerFullPath] = false,    
            [DebugSetting.DrawEnemyControllerNextWaypoint] = false,
            [DebugSetting.DrawEnemyControllerLOS] = false,         
            //[DebugSetting.DrawTowerRange] = false,
            //[DebugSetting.DrawTowerTargetLine] = false,
            //[DebugSetting.DrawTowerCooldown] = false,
            //[DebugSetting.DrawProjectileHomingLine] = false,
            //[DebugSetting.DrawProjectileTargetLine] = false,
            [DebugSetting.ShowEntityBounds] = false,
            [DebugSetting.ShowEntityIDs] = false,
            [DebugSetting.ShowEntityCount] = false,
            [DebugSetting.ShowFPS] = false,
            [DebugSetting.ShowGameTime] = false,
            [DebugSetting.ShowCameraInfo] = false,
            [DebugSetting.ShowInputInfo] = false,
        };

        public DebugSettings() { }

        public void Set(DebugSetting setting, bool value)
        {
            if(settings.ContainsKey(setting))
            {
                settings[setting] = value;
            }
            else
            {
                throw new KeyNotFoundException($"Debug setting '{setting}' not found.");
            }
        }

        public bool Get(DebugSetting setting)
        {
            if(settings.TryGetValue(setting, out bool value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"Debug setting '{setting}' not found.");
            }
        }

        public void Set(string displayString, bool value)
        {
            var setting = DebugSettingExtensions.FromDisplayString(displayString);
            Set(setting, value);
        }

        public bool Get(string displayString)
        {
            var setting = DebugSettingExtensions.FromDisplayString(displayString);
            return Get(setting);
        }

        internal string[] ToArray()
        {
            return DebugSettingExtensions.AllDisplayStrings();
        }

        public bool this[DebugSetting setting]
        {
            get => Get(setting);
            set => Set(setting, value);
        }

        public bool this[string displayString]
        {
            get => Get(displayString);
            set => Set(displayString, value);
        }
    }
}
