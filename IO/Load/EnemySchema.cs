namespace Wc3_Combat_Game.IO.Load.GameSchema
{/*
    public class EnemySchema
    {
        [JsonPropertyName("components")]
        public EnemyComponentSchema Components { get; set; }

        [JsonPropertyName("entities")]
        public List<EntitySchema> Entities { get; set; }
    }

    public class EnemyComponentSchema
    {
        [JsonPropertyName("visual_effects")]
        public Dictionary<string, DrawableSchema> VisualEffects { get; set; }

        [JsonPropertyName("projectiles")]
        public Dictionary<string, ProjectileSchema> Projectiles { get; set; }
    }

    public class DrawableSchema
    {
        [JsonPropertyName("effect_shape")]
        public string EffectShape { get; set; }

        [JsonPropertyName("effect_color")]
        public string EffectColor { get; set; }

        [JsonPropertyName("effect_shape_size")]
        public float EffectShapeSize { get; set; }

        [JsonPropertyName("effect_duration")]
        public float EffectDuration { get; set; }

        [JsonPropertyName("visual_cue_description")]
        public string VisualCueDescription { get; set; }
    }

    public class ProjectileSchema
    {
        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        [JsonPropertyName("visual_effect_ref")]
        public string VisualEffectRef { get; set; }
    
        [JsonIgnore]
        public DrawableSchema VisualEffect { get; set; }
    }

    public class EntitySchema
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("shape")]
        public string Shape { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("stats")]
        public EntityStats Stats { get; set; }

        [JsonPropertyName("melee_attack")]
        public MeleeAttackSchema MeleeAttack { get; set; }

        [JsonPropertyName("ranged_attacks")]
        public List<RangedAttackSchema> RangedAttacks { get; set; }

        [JsonPropertyName("flavor")]
        public string Flavor { get; set; }

        [JsonPropertyName("boss")]
        public bool IsBoss { get; set; }

        [JsonPropertyName("minion_list")]
        public List<string> MinionList { get; set; }
    }

    public class EntityStats
    {
        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("attack")]
        public float Attack { get; set; }

        [JsonPropertyName("health")]
        public float Health { get; set; }

        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        [JsonPropertyName("size")]
        public float Size { get; set; }
    }

    public class MeleeAttackSchema
    {
        [JsonPropertyName("damage_value")]
        public float DamageValue { get; set; }

        [JsonPropertyName("range")]
        public float Range { get; set; }

        [JsonPropertyName("visual_effect_ref")]
        public string VisualEffectRef { get; set; }
    
        [JsonIgnore]
        public DrawableSchema VisualEffect { get; set; }
    }

    public class RangedAttackSchema
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("damage_value")]
        public float DamageValue { get; set; }

        [JsonPropertyName("cooldown")]
        public float Cooldown { get; set; }

        [JsonPropertyName("range")]
        public float Range { get; set; }


        [JsonPropertyName("visual_cue_description")] // Not used.
        public string VisualCueDescription { get; set; } = "";

        [JsonPropertyName("actions")]
        public List<AttackActionSchema> Actions { get; set; } = [];
    }

    public class AttackActionSchema
    {
        [JsonPropertyName("action_type")]
        public string ActionType { get; set; } = "";

        [JsonPropertyName("ref_id")]
        public string RefId { get; set; } = "";
    
        [JsonIgnore]
        public ProjectileSchema? Projectile { get; set; }
    }*/
}