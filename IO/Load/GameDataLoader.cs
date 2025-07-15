namespace Wc3_Combat_Game.IO.Load
{/*
    public static class GameDataLoader
    {
        public static EnemySchema? LoadSchema(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"Error: File not found at {filePath}"); // Use Console.Error for errors
                // Or throw new FileNotFoundException($"The schema file was not found: {filePath}");
                return null;
            }

            string jsonString = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            EnemySchema? schema = null;
            try
            {
                schema = JsonSerializer.Deserialize<EnemySchema>(jsonString, options);
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Error deserializing JSON from {filePath}: {ex.Message}");
                return null;
            }

            ResolveReferences(schema);

            return schema;
        }

        private static void ResolveReferences(EnemySchema schema)
        {
            if (schema == null || schema.Components == null || schema.Entities == null)
            {
                Console.Error.WriteLine("Schema, Components, or Entities are null during reference resolution.");
                return;
            }

            // Resolve VisualEffect references for Projectiles
            foreach (var projectileEntry in schema.Components.Projectiles)
            {
                var projectile = projectileEntry.Value;
                if (schema.Components.VisualEffects.TryGetValue(projectile.VisualEffectRef, out DrawableSchema effect))
                {
                    projectile.VisualEffect = effect;
                }
                else
                {
                    Console.WriteLine($"Warning: Visual effect '{projectile.VisualEffectRef}' not found for projectile '{projectileEntry.Key}'.");
                }
            }

            // Resolve VisualEffect and Projectile references for Entities
            foreach (var entity in schema.Entities)
            {
                // Melee Attack Visual Effect
                if (entity.MeleeAttack != null)
                {
                    if (schema.Components.VisualEffects.TryGetValue(entity.MeleeAttack.VisualEffectRef, out DrawableSchema effect))
                    {
                        entity.MeleeAttack.VisualEffect = effect;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Visual effect '{entity.MeleeAttack.VisualEffectRef}' not found for entity '{entity.Name}' melee attack.");
                    }
                }

                // Ranged Attack Projectile References
                if (entity.RangedAttacks != null)
                {
                    foreach (var rangedAttack in entity.RangedAttacks)
                    {
                        if (rangedAttack.Actions != null)
                        {
                            foreach (var action in rangedAttack.Actions)
                            {
                                if (action.ActionType == "projectile_ref")
                                {
                                    if (schema.Components.Projectiles.TryGetValue(action.RefId, out ProjectileSchema projectile))
                                    {
                                        action.Projectile = projectile;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Warning: Projectile '{action.RefId}' not found for entity '{entity.Name}' ranged attack '{rangedAttack.Name}'.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }*/
}
