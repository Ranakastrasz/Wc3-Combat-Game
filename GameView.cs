using System.Drawing;
using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using static Wc3_Combat_Game.GameController;
using Timer = System.Windows.Forms.Timer;

namespace Wc3_Combat_Game
{

    public partial class GameView : Form
    {
        internal readonly GameController _controller;



        private Player? _mainPlayer;
        private EntityManager<Projectile>? _projectiles;
        private EntityManager<Enemy>? _enemies;

        private float _currentTime;

        private Font _gameOverFont = new Font("Arial", 24, FontStyle.Bold);


        public InputManager Input { get; private set; } = new InputManager();

        private void MainGameWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            Input.OnKeyDown(e.KeyCode);
        }

        private void MainGameWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            Input.OnKeyUp(e.KeyCode);
        }

        private void MainGameWindow_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Input.OnMouseDown(e.Location);
        }

        private void MainGameWindow_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Input.OnMouseUp();
        }

        private void MainGameWindow_MouseMove(object? sender, MouseEventArgs e)
        {
            Input.OnMouseMove(e.Location.ToVector2());
        }

        internal void SetDrawables(Player player, EntityManager<Projectile> projectiles, EntityManager<Enemy> enemies)
        {
            _mainPlayer = player;
            _projectiles = projectiles;
            _enemies = enemies;
        }

        internal GameView(GameController controller)
        {
            InitializeComponent();
            
            _controller = controller;

            this.ClientSize = GameConstants.CLIENT_SIZE.Size;

            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Form gets key events even if controls are focused
            this.Focus();            // Ensure the form has focus


            this.KeyDown += MainGameWindow_KeyDown;
            this.KeyUp += MainGameWindow_KeyUp;

            this.MouseDown += MainGameWindow_MouseDown;
            this.MouseUp += MainGameWindow_MouseUp;
            this.MouseMove += MainGameWindow_MouseMove;


        }


        public void Update(float deltaTime)
        {
            _currentTime = _controller.CurrentTime;

            Point clientPos = this.PointToClient(Control.MousePosition);
            


            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

#if DEBUG
            g.DrawRectangle(Pens.White, GameConstants.GAME_BOUNDS);
            g.DrawRectangle(Pens.Blue, GameConstants.SPAWN_BOUNDS);
#endif

            _mainPlayer?.Draw(g,_currentTime);
            
            _projectiles?.ForEach(p => p.Draw(g, _currentTime));
            _enemies?.ForEach(p => p.Draw(g, _currentTime));

            if (_controller.CurrentState == GameState.GameOver)
            {
                g.DrawString("Game Over", _gameOverFont, Brushes.White, ClientSize.Width/2, ClientSize.Height/2);
            }
        }
        protected void DisposeCustomResources()
        {
            _gameOverFont?.Dispose();
        }

    }
}
