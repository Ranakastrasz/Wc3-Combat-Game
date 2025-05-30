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
    internal class GameManager
    {
        private static GameManager? s_Instance;

        public float GlobalTime { get; private set; } = 0f;
        public static GameManager Instance
        {
            get
            {
                s_Instance ??= new GameManager();
                return s_Instance;
            }
        }



        public Player MainPlayer { get; private set; }


        public List<Projectile> Projectiles { get; private set; } = [];

        public HashSet<Keys> KeysDown { get; set; } = [];
        public bool IsMouseDown { get; set; } = false;
        public bool MouseClicked { get; set; } = false;
        public Point MouseClickedPoint { get; set; } = new(0, 0);
        public List<Enemy> Enemies { get; set; } = [];
        public Vector2 CurrentMousePosition { get; internal set; }

        private float _lastEnemySpawned = 0f;
        private static readonly float ENEMY_SPAWN_COOLDOWN = 0.5f;

        public GameManager()
        {

            // Init player
            MainPlayer = new Player(
                (Vector2)PLAYER_SIZE,
                (Vector2)GAME_BOUNDS.Center(),
                Brushes.Green
            );

        }

        public void Update()
        {
            GlobalTime += FIXED_DELTA_TIME;

            Vector2 move = Vector2.Zero;
            if (KeysDown.Contains(Keys.W)) move.Y -= 1;
            if (KeysDown.Contains(Keys.S)) move.Y += 1;
            if (KeysDown.Contains(Keys.A)) move.X -= 1;
            if (KeysDown.Contains(Keys.D)) move.X += 1;
            if (move != Vector2.Zero)
            {
                move = GeometryUtils.NormalizeAndScale(move, GameConstants.PLAYER_SPEED);
                MainPlayer.InputMove(move);
            }
            //Point mousePoint = this.PointToClient(Cursor.Position);

            if (MouseClicked)
            {
                // Consume the click
                MouseClicked = false;

                // Shoot once at clicked point
                MainPlayer.TryShoot(MouseClickedPoint.ToVector2());
            }
            else if (IsMouseDown)
            {
                // Shoot repeatedly at current mouse position while held (if cooldown allows)
                MainPlayer.TryShoot(CurrentMousePosition);
            }


            if (GlobalTime > _lastEnemySpawned + ENEMY_SPAWN_COOLDOWN)
            {
                _lastEnemySpawned = GlobalTime;
                Enemies.Add(new(
                    (Vector2)ENEMY_SIZE,
                    (Vector2)RandomUtils.RandomPointBorder(GameConstants.SPAWN_BOUNDS),
                    ENEMY_COLOR,
                    MainPlayer,
                    ENEMY_SPEED));
            }

            // Update Entities.

            MainPlayer.Update();
            Projectiles.ForEach(p => p.Update());
            Enemies.ForEach(p => p.Update());


            CheckCollision();

            // Cleanup dead entities.
            Projectiles.RemoveAll(p => p.ToRemove);
            Enemies.RemoveAll(p => p.ToRemove);

            // End Logic
        }

        private void CheckCollision()
        { 
            foreach (Projectile projectile in  Projectiles.Where(p => p.IsAlive)) 
            {
                foreach (Enemy enemy in Enemies.Where(p => p.IsAlive))
                {
                    if (projectile.Intersects(enemy))
                    {
                        projectile.Die();
                        enemy.Damage(50f);
                    }
                }
            }

            foreach (Enemy enemy in Enemies.Where(p => p.IsAlive))
            {
                // Check collision with player.
                if (enemy.Intersects(MainPlayer))
                {
                    // Collision occured.
                    enemy.Die();
                    MainPlayer.Damage(10f);
                }
            }

            foreach (Projectile projectile in Projectiles)
            {
                if (!projectile.BoundingBox.IntersectsWith(GAME_BOUNDS))
                {
                    projectile.Die();
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
