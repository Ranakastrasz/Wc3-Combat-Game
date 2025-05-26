using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using Timer = System.Windows.Forms.Timer;

namespace Wc3_Combat_Game
{

    public partial class MainGameWindow : Form
    {
        private readonly Timer _gameLoopTimer;

        
        private readonly Player _player;
        private readonly float _playerSpeed = 300; // Hardcoded for now.


        private readonly List<Projectile> _projectiles = [];
        private static readonly float _projectileSpeed = 1200;
        private static readonly float _projectileLifespan = 0.25f;

        //private List<System.Drawing.Rectangle> enemies; // tbi

        public MainGameWindow()
        {
            InitializeComponent();

            // Setup game loop timer
            _gameLoopTimer = new() { Interval = GameConstants.TickDurationMs };
            _gameLoopTimer.Tick += GameLoopTimer_Tick;
            _gameLoopTimer.Start();

            // Init player
            _player = new Player(new Vector2(25, 25), new Vector2(25, 25), Brushes.Green);


            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Form gets key events even if controls are focused
            this.Focus();            // Ensure the form has focus

            this.KeyDown += MainGameWindow_KeyDown;
            this.KeyUp += MainGameWindow_KeyUp;
            this.MouseDown += MainGameWindow_MouseDown;
            //this.MouseUp += MainGameWindow_MouseUp; Later, for holding the button to repeat fire.

        }

        #region KeyInput
        // Note. Cannot capture multiple rapid keypresses.
        // Given I am using this to detect holding, this doesnt matter much.
        // Doesn't detect keystrokes though if they happen between frames.
        // I think. So won't work for hotkeys. Probably want a class to manage this.
        

        private readonly HashSet<Keys> _keysDown = [];

        private void MainGameWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            _keysDown.Add(e.KeyCode);
        }

        private void MainGameWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            _keysDown.Remove(e.KeyCode);
        }
        #endregion

        #region Mouse Input and State
        // Same issue about lack of buffering. This gets a single click and its position between frames.
        // Will not support multiple clicks in a frame, though to be fair, I doubt this is a big deal

        bool _mouseClicked = false;
        Point _mouseClickedPoint = new(0, 0);
        private void MainGameWindow_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Left button pressed at e.Location
                _mouseClicked = true;
                _mouseClickedPoint = e.Location;
            }
        }
        #endregion


        private void GameLoopTimer_Tick(object? sender, EventArgs e)
        {
            Vector2 move = Vector2.Zero;
            if (_keysDown.Contains(Keys.W)) move.Y -= 1;
            if (_keysDown.Contains(Keys.S)) move.Y += 1;
            if (_keysDown.Contains(Keys.A)) move.X -= 1;
            if (_keysDown.Contains(Keys.D)) move.X += 1;
            if (move != Vector2.Zero)
            {
                move = GeometryUtil.NormalizeAndScale(move, _playerSpeed);
                _player.InputMove(move);
                //move = move.NormalizeAndScale(_playerSpeed*GameConstants.FixedDeltaTime);
                //_player.Position += move;
            }

            //Point mousePoint = this.PointToClient(Cursor.Position);
            // for active mouse tracking later.

            if (_mouseClicked)
            {
                Vector2 velocity = Vector2.Normalize(_mouseClickedPoint.ToVector2() - _player.Position) * _projectileSpeed;


                Projectile projectile = new(new(10, 10), _player.Position, Brushes.Blue, velocity,_projectileLifespan);

                // add to the list
                _projectiles.Add(projectile);

                //_mouseClicked = false; // Reset Mouse Click event.
            }

            _player.Update();
            _projectiles.ForEach(p => p.Update());
            _projectiles.RemoveAll(p => p.TimeToLive <= 0);

            // End Logic

            this.Invalidate(); // Trigger OnPaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            _player.Draw(g);

            _projectiles.ForEach(p => p.Draw(g));
        }
    }
}
