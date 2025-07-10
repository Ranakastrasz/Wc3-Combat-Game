using AssertUtils;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Windows.Forms;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Wc3_Combat_Game.Core.GameController;

namespace Wc3_Combat_Game
{

    public partial class GameView: Form
    {
        internal readonly GameController _controller;

        private Camera _camera;

        public IDrawContext? DrawContext { get; private set; }

        private Font? _gridFont;

        // In GameView.cs, add these:
        private readonly Stopwatch _paintStopwatch = new Stopwatch();
        public readonly object PaintDurationsLock = new object();
        public Queue<double> DebugPaintDurations = new Queue<double>(60); // For rendering durations

        #region InputHooks
        public InputManager Input { get; private set; } = new InputManager();

        public bool DebugPanelVisible => DebugPanel.Visible;

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
            if(e.Button == MouseButtons.Left)
                Input.OnMouseDown(ScreenToWorld(e.Location));
        }

        private void MainGameWindow_MouseUp(object? sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                Input.OnMouseUp();
        }

        private void MainGameWindow_MouseMove(object? sender, MouseEventArgs e)
        {
            Input.OnMouseMove(ScreenToWorld(e.Location));
        }
        #endregion

        public Vector2 ScreenToWorld(Point screenPos)
        {
            var matrix = _camera.GetTransform();
            matrix.Invert();
            var points = new PointF[] { new PointF(screenPos.X, screenPos.Y) };
            matrix.TransformPoints(points);
            return new Vector2(points[0].X, points[0].Y);
        }

        internal GameView(GameController controller, IDrawContext context)
        {
            InitializeComponent();

            _controller = controller;
            DrawContext = context;

            //this.ClientSize = GameConstants.CAMERA_SCALE;
            this._camera = new Camera();
            _camera.LerpFactor = 10f;

            _camera.Zoom = 3;
            _camera.Width = GameConstants.CAMERA_SCALE.Width;
            _camera.Height = GameConstants.CAMERA_SCALE.Height;

            // In GameView constructor:
            if(GameWindow != null)
            {
                _camera.Width = GameWindow.ClientSize.Width;
                _camera.Height = GameWindow.ClientSize.Height;
            }
            //AssertUtil.NotNull(_drawContext.PlayerUnit);
            //_camera.FollowUnit(_drawContext.PlayerUnit!);


            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Form gets key events even if controls are focused
            this.Focus();            // Ensure the form has focus


            this.KeyDown += MainGameWindow_KeyDown;
            this.KeyUp += MainGameWindow_KeyUp;

            this.MouseDown += MainGameWindow_MouseDown;
            this.MouseUp += MainGameWindow_MouseUp;
            this.MouseMove += MainGameWindow_MouseMove;

            AssertUtil.NotNull(GameWindow);
            GameWindow.KeyDown += MainGameWindow_KeyDown;
            GameWindow.KeyUp += MainGameWindow_KeyUp;
            GameWindow.MouseDown += MainGameWindow_MouseDown;
            GameWindow.MouseUp += MainGameWindow_MouseUp;
            GameWindow.MouseMove += MainGameWindow_MouseMove;

            DebugPanel.Visible = false; // Hidden by default, toggle with F4.

            AssertUtil.NotNull(DebugPathfinding);
            DebugPathfinding.Items.AddRange(DrawContext.DebugSettings.ToArray());
            //DebugPathfinding.Items.AddRange(
            //    ["Draw.Enemy.Controller.State",
            //    "Draw.Enemy.MovementVector",
            //    "Draw.Enemy.FullPath",
            //    "Draw.Enemy.NextWaypoint",
            //    "Draw.Enemy.CollisionBox",
            //    "Draw.Enemy.LOS"]);



            this.Resize += OnWindowSizeChanged;

            _gridFont = null;
            this.KeyPreview = true; // For debug form purposes.

        }

        private void GamePanel_Paint(object? sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowSizeChanged(object? sender, EventArgs e)
        {
            // Get the size of the GameWindow panel, not the Form's client area
            var bounds = GameWindow.ClientRectangle;

            // Target aspect ratio (from GameConstants.CAMERA_BOUNDS)
            float targetAspect = (float)GameConstants.CAMERA_SCALE.Width / GameConstants.CAMERA_SCALE.Height;
            float windowAspect = (float)bounds.Width / bounds.Height;

            int newWidth, newHeight;

            if(windowAspect > targetAspect)
            {
                // Window is wider than target: pillarbox
                newHeight = bounds.Height;
                newWidth = (int)(newHeight * targetAspect);
            }
            else
            {
                // Window is taller than target: letterbox
                newWidth = bounds.Width;
                newHeight = (int)(newWidth / targetAspect);
            }

            // Set camera to fit inside GameWindow, preserving aspect ratio
            _camera.Width = newWidth;
            _camera.Height = newHeight;
            _camera.SnapToUnit();
        }
        public void Update(float deltaTime)
        {
            _camera.Update(deltaTime);

            GameWindow.Invalidate();
            DebugWaveChart.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {

        }

        private void GameWindow_Paint(object sender, PaintEventArgs e)
        {
            _paintStopwatch.Restart(); // Start measuring

            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            // Apply camera transform
            using var transform = _camera.GetTransform();
            g.Transform = transform;

            Rectangle clientRect = GameWindow.ClientRectangle;

            // Draw Map.
            if(DrawContext != null)
            {
                Map? map = DrawContext.Map;
                AssertUtil.NotNull(map);


                float tileSize = map.TileSize;

                for(int y = 0; y < map.TileMap.GetLength(1); y++)
                {
                    for(int x = 0; x < map.TileMap.GetLength(0); x++)
                    {
                        Tile tile = map.TileMap[x, y];
                        Brush brush = new SolidBrush(tile.GetColor); // optionally cache these

                        //g.FillRectangle(brush,x*tileSize,y*tileSize,tileSize,tileSize);
                        // Tell tiles to draw themselves.
                        tile.Draw(g, tileSize);

                        brush.Dispose(); // only if not cached
                    }
                }

            }

            DrawContext?.Entities?.ForEach(p => p.Draw(g, DrawContext));


            if(_controller.CurrentState == GameState.GameOver || _controller.CurrentState == GameState.Victory)
            {
                g.ResetTransform();

                using var gameOverFont = new Font("Arial", 24, FontStyle.Bold);
                string message = _controller.CurrentState == GameState.GameOver ? "Game Over" : "Victory!";
                SizeF textSize = g.MeasureString(message, gameOverFont);

                float x = (ClientSize.Width - textSize.Width) / 2;
                float y = (ClientSize.Height - textSize.Height) / 2;

                g.DrawString(message, gameOverFont, Brushes.White, x, y);


            }

            if(_controller.IsPaused())
            {
                // Draw a semi-transparent overlay to indicate pause state
                using var overlayBrush = new SolidBrush(Color.FromArgb(128, Color.Gray));
                g.FillRectangle(overlayBrush, this.ClientRectangle);
            }

            // Safely enqueue the elapsed paint time
            _paintStopwatch.Stop();
            double elapsed = _paintStopwatch.Elapsed.TotalMilliseconds;
            lock(PaintDurationsLock)
            {
                DebugPaintDurations.Enqueue(elapsed);
                if(DebugPaintDurations.Count > 60) // Keep reasonable size
                {
                    DebugPaintDurations.Dequeue();
                }
            }
        }

        public void RegisterPlayer(Unit playerUnit)
        {
            _camera.FollowUnit(playerUnit);
            _camera.SnapToUnit(playerUnit);
        }

        private void DisposeCustomResources() => _gridFont?.Dispose();

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.Escape)
            {
                // Handle Escape key to close the game
                Close(); // or Application.Exit() if you want to exit the application
                return true; // Indicate that we've handled this key press
            }
            if(keyData == Keys.P)
            {
                // Handle P key to toggle pause state
                _controller.TogglePause(); // Toggle game pause state
                return true; // Indicate that we've handled this key press
            }
            if(keyData == Keys.F4)
            {
                DebugPanel.Visible = !DebugPanel.Visible; // Toggle the debug panel visibility
                return true; // Indicate that we've handled this key press
            }
            return base.ProcessCmdKey(ref msg, keyData); // Let base class handle other keys
        }        // This method handles F4 when the DebugSettingsForm has focus and raises its event

        private void DebugPathfinding_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TextBox_DebugOutput.Text = "Item with title \"" + DebugPathfinding.Items[e.Index].ToString() +
        "\" was checked. The new check state is " + e.NewValue.ToString();
            string itemText = DebugPathfinding.Items[e.Index].ToString() ?? "";
            bool isChecked = e.NewValue == CheckState.Checked;
            DrawContext?.DebugSettings.Set(itemText, isChecked);

        }

        private void DebugWaveChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // Clear the background
            g.Clear(Color.LightGray);

            if (DrawContext.DebugSettings.Get(DebugSetting.ShowFPS))
            {
                g.DrawString($"FPS: {_controller.Fps}", new Font("Arial", 10), Brushes.Black, new PointF(10, 10));
                //g.DrawString($"FPS: {_controller.Fps}", new Font("Arial", 10), Brushes.Black, new PointF(10, 30));
            }


            // Safely get a snapshot of the data
            double[] TickData;
            double[] PaintData;
            lock(_controller.TickDurationsLock) // Use the lock object from the controller
            {
                TickData = _controller.DebugTickDurations.ToArray();
            }
            lock(PaintDurationsLock) // Use the lock object from the GameView
            {
                PaintData = DebugPaintDurations.ToArray();
            }   

            // Define drawing parameters
            Pen pen = new Pen(Color.Blue, 1);
            int panelWidth = e.ClipRectangle.Width;
            int panelHeight = e.ClipRectangle.Height;

            // Find min/max values for scaling (e.g., for SimDeltaTime)
            double maxTickDuration = 0f;
            if(TickData.Length > 0)
            {
                maxTickDuration = TickData.Max(); // Using LINQ's Max() for convenience
            }
            // Ensure maxTickDuration is not zero to avoid division by zero
            if(maxTickDuration == 0) maxTickDuration = GameConstants.TICK_DURATION_MS * 2; // Default if no data or all zeros

            // Calculate horizontal scaling (each data point takes up a certain width)
            double x_scale = (double)panelWidth / TickData.Length;

            // Draw the waveform
            for(int i = 0; i < TickData.Length - 1; i++)
            {
                double p1 = TickData[i];
                double p2 = TickData[i + 1];

                // X coordinates are based on index
                double x1 = i * x_scale;
                double x2 = (i + 1) * x_scale;

                // Y coordinates are based on tick duration, scaled to panel height
                // Invert Y-axis so higher values are drawn higher on the panel (Windows Forms Y-axis is top-down)
                double y1 = panelHeight - (p1 / maxTickDuration) * panelHeight;
                double y2 = panelHeight - (p2 / maxTickDuration) * panelHeight;

                g.DrawLine(pen, (int)x1, (int)y1, (int)x2, (int)y2);
            }

            pen.Dispose();


            // Define drawing parameters
            pen = new Pen(Color.Red, 1);

            // Find min/max values for scaling (e.g., for SimDeltaTime)
            maxTickDuration = 0f;
            if(PaintData.Length > 0)
            {
                maxTickDuration = PaintData.Max(); // Using LINQ's Max() for convenience
            }
            // Ensure maxTickDuration is not zero to avoid division by zero
            if(maxTickDuration == 0) maxTickDuration = GameConstants.TICK_DURATION_MS * 2; // Default if no data or all zeros

            // Calculate horizontal scaling (each data point takes up a certain width)
            x_scale = (double)panelWidth / PaintData.Length;

            // Draw the waveform
            for(int i = 0; i < PaintData.Length - 1; i++)
            {
                double p1 = PaintData[i];
                double p2 = PaintData[i + 1];

                // X coordinates are based on index
                double x1 = i * x_scale;
                double x2 = (i + 1) * x_scale;

                // Y coordinates are based on tick duration, scaled to panel height
                // Invert Y-axis so higher values are drawn higher on the panel (Windows Forms Y-axis is top-down)
                double y1 = panelHeight - (p1 / maxTickDuration) * panelHeight;
                double y2 = panelHeight - (p2 / maxTickDuration) * panelHeight;

                g.DrawLine(pen, (int)x1, (int)y1, (int)x2, (int)y2);
            }

            pen.Dispose();
        }
    }
}
