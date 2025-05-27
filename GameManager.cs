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
        public bool MouseClicked { get; set; } = false;
        public Point MouseClickedPoint { get; set; } = new(0, 0);
        public List<Enemy> Enemies { get; set; } = [];

        private float _enemySpawnTimer = ENEMY_SPAWN_TIMER;

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
                Vector2 velocity = Vector2.Normalize(MouseClickedPoint.ToVector2() - MainPlayer.Position) * PROJECTILE_SPEED;


                Projectile projectile = new((Vector2)PROJECTILE_SIZE,
                    MainPlayer.Position,
                    PROJECTILE_COLOR,
                    velocity,
                    PROJECTILE_LIFESPAN);

                // add to the list
                Projectiles.Add(projectile);

                MouseClicked = false; // Consume Mouseclick.
                
            }

            _enemySpawnTimer -= FIXED_DELTA_TIME;
            if (_enemySpawnTimer < 0)
            {
                _enemySpawnTimer = ENEMY_SPAWN_TIMER;
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
            Projectiles.RemoveAll(p => !p.IsAlive);
            Enemies.RemoveAll(p => !p.IsAlive);

            // End Logic
        }

        private void CheckCollision()
        { 
            foreach (Projectile projectile in  Projectiles) 
            {
                foreach (Enemy enemy in Enemies)
                { 
                    if (projectile.Intersects(enemy))
                    {
                        projectile.Die();
                        enemy.Die();
                    }
                }
            }

            foreach (Enemy enemy in Enemies)
            {
                // Check collision with player.
                if (enemy.Intersects(MainPlayer))
                {
                    // Collision occured.
                }
            }

            foreach (Projectile projectile in Projectiles)
            {
                // Check out of bounds
            }
        }

    }
}
