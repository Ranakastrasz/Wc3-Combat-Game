using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototypes;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Effects;
using Timer = System.Windows.Forms.Timer;
using static Wc3_Combat_Game.GameConstants;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Interface;

namespace Wc3_Combat_Game
{
    internal class GameSession
    {
        private readonly GameController _controller;

        public float CurrentTime { get; private set; } = 0f;


        // Entities.
        public Unit MainPlayer { get; private set; }

        public EntityManager<Projectile> Projectiles { get; private set; } = new();
        public EntityManager<Unit> Units { get; private set; } = new();


        private float _lastEnemySpawned = 0f;


        public GameSession(GameController controller)
        {
            _controller = controller;

            // Init player
            MainPlayer = new(
                (Vector2)PLAYER_SIZE,
                (Vector2)GAME_BOUNDS.Center(),
                100f,
                GameConstants.PLAYER_SPEED,
                Brushes.Green

            )
            {
                Controller = new IPlayerController(controller.Input),
                Weapon = new IBasicWeapon(new EffectProjectile(PrototypeDefines.PLAYER_PROJECTILE, Projectiles),GameConstants.PLAYER_COOLDOWN, float.PositiveInfinity),
                Team = TeamType.Ally                
            };
            Units.Add(MainPlayer);

        }




        private void CheckGameOverCondition(float currentTime)
        {
            if (MainPlayer.IsExpired(currentTime))
            {
                _controller.OnGameOver();
            }
        }

        public void Update(float deltaTime)
        {
            CurrentTime += deltaTime;
            

            if (CurrentTime > _lastEnemySpawned + ENEMY_SPAWN_COOLDOWN)
            {
                _lastEnemySpawned = CurrentTime;
                Unit unit = new(
                    (Vector2)ENEMY_SIZE,
                    (Vector2)RandomUtils.RandomPointBorder(GameConstants.SPAWN_BOUNDS),

                    10f,
                    GameConstants.ENEMY_SPEED,
                    ENEMY_COLOR)
                {
                    Target = MainPlayer,
                    Controller = new IBasicAIController(),
                    Weapon = new IBasicWeapon(new EffectProjectile(PrototypeDefines.ENEMY_PROJECTILE, Projectiles),1f,100f),
                    Team = TeamType.Enemy
                };

                Units.Add(unit);

                
            }

            // Update Entities.

            MainPlayer.Update(deltaTime, CurrentTime);
            Projectiles.UpdateAll(deltaTime, CurrentTime);
            Units.UpdateAll(deltaTime, CurrentTime);

            CheckCollision(deltaTime);


            // Cleanup dead entities.
            Projectiles.RemoveExpired(CurrentTime);
            Units.RemoveExpired(CurrentTime);


            CheckGameOverCondition(CurrentTime);
            // End Logic
        }

        private void CheckCollision(float deltaTime)
        {
            foreach (Projectile projectile in Projectiles.Entities.Where(p => p.IsAlive))
            {
                foreach (Unit unit in Units.Entities.Where(p => p.IsAlive && p.Team.IsHostileTo(projectile.Team)))
                {
                    if (projectile.Intersects(unit))
                    {
                        projectile.Die(CurrentTime);
                        projectile.ImpactEffect.ApplyToEntity(projectile.Caster, projectile, unit, CurrentTime);

                        break;
                    }
                }
            }

            foreach (Projectile projectile in Projectiles.Entities)
            {
                if (!projectile.BoundingBox.IntersectsWith(GAME_BOUNDS))
                {
                    projectile.Die(CurrentTime);
                }
            }

            if (!GameConstants.GAME_BOUNDS.Contains(MainPlayer.BoundingBox))
            {
                var bounds = GameConstants.GAME_BOUNDS;
                var halfSize = new SizeF(MainPlayer.Size.X / 2f, MainPlayer.Size.Y / 2f);

                MainPlayer.Position = new Vector2(
                    Math.Clamp(MainPlayer.Position.X, bounds.Left + halfSize.Width, bounds.Right - halfSize.Width),
                    Math.Clamp(MainPlayer.Position.Y, bounds.Top + halfSize.Height, bounds.Bottom - halfSize.Height)
                );
            }
        }

    }
}
