using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using Timer = System.Windows.Forms.Timer;

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
        private readonly float _playerSpeed = 300; // Hardcoded for now.


        public List<Projectile> Projectiles { get; private set; } = [];
        private static readonly float s_ProjectileSpeed = 1200;
        private static readonly float s_ProjectileLifespan = 0.25f;

        public HashSet<Keys> KeysDown { get; set; } = [];
        public bool MouseClicked { get; set; } = false;
        public Point MouseClickedPoint { get; set; } = new(0, 0);


        //private List<System.Drawing.Rectangle> enemies; // tbi

        public GameManager()
        {

            // Init player
            MainPlayer = new Player(new Vector2(25, 25), new Vector2(25, 25), Brushes.Green);

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
                move = GeometryUtil.NormalizeAndScale(move, _playerSpeed);
                MainPlayer.InputMove(move);
                //move = move.NormalizeAndScale(_playerSpeed*GameConstants.FixedDeltaTime);
                //_player.Position += move;
            }

            //Point mousePoint = this.PointToClient(Cursor.Position);
            // for active mouse tracking later.

            if (MouseClicked)
            {
                Vector2 velocity = Vector2.Normalize(MouseClickedPoint.ToVector2() - MainPlayer.Position) * s_ProjectileSpeed;


                Projectile projectile = new(new(10, 10), MainPlayer.Position, Brushes.Blue, velocity, s_ProjectileLifespan);

                // add to the list
                Projectiles.Add(projectile);

                MouseClicked = false; // Reset Mouse Click event.
                
            }

            MainPlayer.Update();
            Projectiles.ForEach(p => p.Update());
            Projectiles.RemoveAll(p => p.TimeToLive <= 0);

            // End Logic
        }

    }
}
