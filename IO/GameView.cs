using System.Drawing;
using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;
using static Wc3_Combat_Game.Core.GameController;

namespace Wc3_Combat_Game
{

    public partial class GameView : Form
    {
        internal readonly GameController _controller;



        private EntityManager<Projectile>? _projectiles;
        private EntityManager<Unit>? _units;

        private BoardContext? _context;

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

        internal void SetDrawables(EntityManager<Projectile> projectiles, EntityManager<Unit> enemies)
        {
            _projectiles = projectiles;
            _units = enemies;
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


        public void Update(float deltaTime, BoardContext context)
        {
            _context = context;

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
            if (_context != null)
            {
                _projectiles?.ForEach(p => p.Draw(g, _context));
                _units?.ForEach(p => p.Draw(g, _context));
            }
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
