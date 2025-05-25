using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace Wc3_Combat_Game
{

    public partial class MainGameWindow : Form
    {
        private readonly Timer _gameLoopTimer;

        class Player(Vector2 size, Vector2 position)
        {
            private Vector2 _size = size;
            private Vector2 _position = position;
            // Velocity, cooldown, health, mana, etc.
            // Later
            public Rectangle DrawRect
            {
                get => _size.RectFromCenter(_position);
                /*new(
                    (int)(Position.X - Size.X / 2), (int)(Position.Y - Size.Y / 2),
                    (int)Size.X, (int)Size.Y
                );*/
            }
            public Vector2 Size { get => _size; set => _size = value; }
            public Vector2 Position { get => _position; set => _position = value; }
        }
        private readonly Player _player;
        private readonly int _playerSpeed = 5; // Hardcoded for now.

        // Projectiles have a rect, general speed, plus their current velocity.
        class Projectile(Vector2 size, Vector2 position, Vector2 velocity, int timeToLive)
        {
            public Vector2 _size = size;
            private Vector2 _position = position;
            private Vector2 _velocity = velocity;
            private int _timeToLive = timeToLive;

            public Rectangle DrawRect // Candidate for Entity base class if more units are added.

            {
                get => _size.RectFromCenter(_position);
            }
            public Vector2 Position { get => _position; set => _position = value; }
            public Vector2 Velocity { get => _velocity; set => _velocity = value; }
            public int TimeToLive { get => _timeToLive; set => _timeToLive = value; }
        }

        private readonly List<Projectile> _projectiles = [];
        private static readonly int _projectileSpeed = 10;
        private static readonly int _projectileLifespan = 60 * 2;

        //private List<System.Drawing.Rectangle> enemies; // tbi

        public MainGameWindow()
        {
            InitializeComponent();

            // Setup game loop timer
            _gameLoopTimer = new() { Interval = 16 }; // ~60 FPS
            _gameLoopTimer.Tick += GameLoopTimer_Tick;
            _gameLoopTimer.Start();

            // Init player
            _player = new Player(new Vector2(100, 100), new Vector2(25, 25));


            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Form gets key events even if controls are focused
            this.Focus();            // Ensure the form has focus

            this.KeyDown += MainGameWindow_KeyDown;
            this.KeyUp += MainGameWindow_KeyUp;
            this.MouseDown += MainGameWindow_MouseDown;

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
                move.NormalizeAndScale(_playerSpeed);
                _player.Position += move;
            }

            //Point mousePoint = this.PointToClient(Cursor.Position);
            // for active mouse tracking.

            if (_mouseClicked)
            {
                Vector2 velocity = Vector2.Normalize(_mouseClickedPoint.ToVector2() - _player.Position) * _projectileSpeed;


                Projectile projectile = new(new(25, 25), _player.Position, velocity,_projectileLifespan);

                // add to the list
                _projectiles.Add(projectile);

                _mouseClicked = false; // Reset Mouse Click event.
            }

            _projectiles.RemoveAll(p => --p.TimeToLive <= 0);

            _projectiles.ForEach(p => p.Position += p.Velocity);
            //foreach (Projectile projectile in _projectiles)
            //{
            //    projectile._position += projectile.Velocity;
            //}

            // End Logic

            this.Invalidate(); // Trigger OnPaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            g.FillRectangle(Brushes.Green, _player.DrawRect);

            foreach (Projectile projectile in _projectiles)
            {
                g.FillRectangle(Brushes.Blue, projectile.DrawRect);
            }
        }
    }
}
