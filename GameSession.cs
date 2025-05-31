using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using Timer = System.Windows.Forms.Timer;
using static Wc3_Combat_Game.GameConstants;

namespace Wc3_Combat_Game
{
    internal class GameSession
    {
        private readonly GameController _controller;

        public float CurrentTime { get; private set; } = 0f;


        // Entities.
        public Player MainPlayer { get; private set; }

        public EntityManager<Projectile> Projectiles { get; private set; } = new();
        public EntityManager<Enemy> Enemies { get; private set; } = new();


        private float _lastEnemySpawned = 0f;


        public GameSession(GameController controller)
        {
            _controller = controller;

            // Init player
            MainPlayer = new Player(
                (Vector2)PLAYER_SIZE,
                (Vector2)GAME_BOUNDS.Center(),
                Brushes.Green

            );


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
            InputManager input = _controller.Input;


            Vector2 move = Vector2.Zero;
            if (input.IsKeyDown(Keys.W)) move.Y -= 1;
            if (input.IsKeyDown(Keys.S)) move.Y += 1;
            if (input.IsKeyDown(Keys.A)) move.X -= 1;
            if (input.IsKeyDown(Keys.D)) move.X += 1;
            if (move != Vector2.Zero)
            {
                move = GeometryUtils.NormalizeAndScale(move, GameConstants.PLAYER_SPEED);
                MainPlayer.InputMove(move);
            }
            //Point mousePoint = this.PointToClient(Cursor.Position);

            if (input.IsMouseClicked())
            {

                // Shoot once at clicked point
                MainPlayer.TryShoot(input.MouseClickedPosition, CurrentTime, Projectiles);
            }
            else if (input.IsMouseDown())
            {
                // Shoot repeatedly at current mouse position while held (if cooldown allows)
                MainPlayer.TryShoot(input.CurrentMousePosition, CurrentTime, Projectiles);
            }


            if (CurrentTime > _lastEnemySpawned + ENEMY_SPAWN_COOLDOWN)
            {
                _lastEnemySpawned = CurrentTime;
                Enemies.Add(new(
                    (Vector2)ENEMY_SIZE,
                    (Vector2)RandomUtils.RandomPointBorder(GameConstants.SPAWN_BOUNDS),
                    ENEMY_COLOR,
                    MainPlayer,
                    ENEMY_SPEED));
            }

            // Update Entities.

            MainPlayer.Update(deltaTime, CurrentTime);
            Projectiles.UpdateAll(deltaTime, CurrentTime);
            Enemies.UpdateAll(deltaTime, CurrentTime);

            CheckCollision(deltaTime);


            // Cleanup dead entities.
            Projectiles.RemoveExpired(CurrentTime);
            Enemies.RemoveExpired(CurrentTime);


            CheckGameOverCondition(CurrentTime);
            // End Logic
        }

        private void CheckCollision(float deltaTime)
        { 
            foreach (Projectile projectile in  Projectiles.Entities.Where(p => p.IsAlive)) 
            {
                foreach (Enemy enemy in Enemies.Entities.Where(p => p.IsAlive))
                {
                    if (projectile.Intersects(enemy))
                    {
                        projectile.Die(CurrentTime);
                        enemy.Damage(50f, CurrentTime);
                    }
                }
            }

            foreach (Enemy enemy in Enemies.Entities.Where(p => p.IsAlive))
            {
                // Check collision with player.
                if (enemy.Intersects(MainPlayer))
                {
                    // Collision occured.
                    enemy.Die(CurrentTime);
                    MainPlayer.Damage(50f, CurrentTime);
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
