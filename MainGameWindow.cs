using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using Timer = System.Windows.Forms.Timer;

namespace Wc3_Combat_Game
{

    public partial class MainGameWindow : Form
    {

        private readonly Timer _gameLoopTimer;

        public MainGameWindow()
        {
            InitializeComponent();
            /*
                Rectangle rect = new Rectangle(0, 0, desiredClientWidth, desiredClientHeight);
                AdjustWindowRectEx(ref rect, (uint)CreateParams.Style, hasMenu, (uint)CreateParams.ExStyle);
                form.Size = new Size(rect.Width - rect.X, rect.Height - rect.Y);
             */
            this.ClientSize = GameConstants.CLIENT_SIZE.Size;

            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Form gets key events even if controls are focused
            this.Focus();            // Ensure the form has focus


            this.KeyDown += MainGameWindow_KeyDown;
            this.KeyUp += MainGameWindow_KeyUp;
            this.MouseDown += MainGameWindow_MouseDown;
            this.MouseUp += MainGameWindow_MouseUp; //Later, for holding the button to repeat fire.

            // Setup game loop timer
            _gameLoopTimer = new() { Interval = GameConstants.TICK_DURATION_MS };
            _gameLoopTimer.Tick += GameLoopTimer_Tick;
            _gameLoopTimer.Start();

        }

        #region KeyInput
        // Note. Cannot capture multiple rapid keypresses.
        // Given I am using this to detect holding, this doesnt matter much.
        // Doesn't detect keystrokes though if they happen between frames.
        // I think. So won't work for hotkeys. Probably want a class to manage this.
        


        private void MainGameWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            // This is too intrusive I think.
            GameManager.Instance.KeysDown.Add(e.KeyCode);
        }

        private void MainGameWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            GameManager.Instance.KeysDown.Remove(e.KeyCode);
        }
        #endregion

        #region Mouse Input and State
        // Same issue about lack of buffering. This gets a single click and its position between frames.
        // Will not support multiple clicks in a frame, though to be fair, I doubt this is a big deal

        private void MainGameWindow_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Left button pressed at e.Location
                GameManager.Instance.MouseClicked = true;
                GameManager.Instance.MouseClickedPoint = e.Location;
            }
        }
        private void MainGameWindow_MouseUp(object? sender, MouseEventArgs e)
        {
            // Not yet implemented.
            //if (e.Button == MouseButtons.Left)
            //{
            //    // Left button pressed at e.Location
            //    _mouseClicked = true;
            //    _mouseClickedPoint = e.Location;
            //}
        }
        #endregion




        private void GameLoopTimer_Tick(object? sender, EventArgs e)
        {
            // Capture keys/mouse state, and pass input into the gameManager.Update ?
            
            GameManager.Instance.Update();


            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            g.DrawRectangle(Pens.Green, GameConstants.PLAYER_BOUNDS);
            g.DrawRectangle(Pens.Red, GameConstants.CULL_BOUNDS);
            g.DrawRectangle(Pens.White, GameConstants.GAME_BOUNDS);
            g.DrawRectangle(Pens.Blue, GameConstants.SPAWN_BOUNDS);

            GameManager.Instance.MainPlayer.Draw(g);
            GameManager.Instance.Projectiles.ForEach(p => p.Draw(g));
            GameManager.Instance.Enemies.ForEach(p => p.Draw(g));


        }
    }
}
