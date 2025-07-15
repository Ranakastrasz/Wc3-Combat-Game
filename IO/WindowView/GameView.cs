using AssertUtils;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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


        // In GameView.cs, add these:
        private readonly Stopwatch _paintStopwatch = new Stopwatch();
        public readonly object PaintDurationsLock = new object();
        public Queue<double> DebugPaintDurations = new Queue<double>(60); // For rendering durations

        #region InputHooks
        public InputManager Input { get; private set; } = new InputManager();

        public bool DebugPanelVisible => DebugPanel.Visible;

        public Camera Camera { get => _camera; set => _camera = value; }

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
            _camera = new Camera();
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

            this.KeyPreview = true; // For debug form purposes.

        }

        private void GamePanel_Paint(object? sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowSizeChanged(object? sender, EventArgs e)
        {
            //// Get the size of the GameWindow panel, not the Form's client area
            //var bounds = GameWindow.ClientRectangle;
            //
            //// Target aspect ratio (from GameConstants.CAMERA_BOUNDS)
            //float targetAspect = (float)GameConstants.CAMERA_SCALE.Width / GameConstants.CAMERA_SCALE.Height;
            //float windowAspect = (float)bounds.Width / bounds.Height;
            //
            //int newWidth, newHeight;
            //
            //if(windowAspect > targetAspect)
            //{
            //    // Window is wider than target: pillarbox
            //    newHeight = bounds.Height;
            //    newWidth = (int)(newHeight * targetAspect);
            //}
            //else
            //{
            //    // Window is taller than target: letterbox
            //    newWidth = bounds.Width;
            //    newHeight = (int)(newWidth / targetAspect);
            //}
            //
            //// Set camera to fit inside GameWindow, preserving aspect ratio
            //_camera.Width = newWidth;
            //_camera.Height = newHeight;
            _camera.Width = GameWindow.ClientSize.Width;
            _camera.Height = GameWindow.ClientSize.Height;

            _camera.SnapToUnit();
        }
        public void Update(float deltaTime)
        {
            _camera.Update(deltaTime);

            GameWindow.Invalidate();
            if (DrawContext != null && DrawContext.DebugSettings.Get(DebugSetting.ShowFPS)) // not 100% FPS, but eh, close enough.
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

            // Draw Map.
            if(DrawContext != null)
            {
                Map? map = DrawContext.Map;
                AssertUtil.NotNull(map);

                map.Draw(g, DrawContext);
            }


            //Rectangle clientRect = GameWindow.ClientRectangle;


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

            AssertUtil.NotNull(DrawContext);
            if (DrawContext.DebugSettings.Get(DebugSetting.ShowCameraBounds))
            {
                // Draw camera bounds in world space (inverted Y for Windows Forms)
                var cameraBounds = _camera.Viewport;
                var cameraPen = DrawContext.DrawCache.GetPen(Color.Magenta, 2);
                g.DrawRectangle(cameraPen, cameraBounds.X, cameraBounds.Y, cameraBounds.Width, cameraBounds.Height);
            }

            if(_controller.IsPaused())
            {
                // Draw a semi-transparent overlay to indicate pause state

                var overlayBrush = DrawContext.DrawCache.GetSolidBrush(Color.FromArgb(128, Color.Gray));
                g.Transform = new Matrix(); // Reset Transform to Identity.
                g.FillRectangle(overlayBrush, this.ClientRectangle);
            }

            // Safely enqueue the elapsed paint time
            _paintStopwatch.Stop();
            double elapsed = _paintStopwatch.Elapsed.TotalMilliseconds;
            lock(PaintDurationsLock)
            {
                DebugPaintDurations.Enqueue(elapsed);
                if(DebugPaintDurations.Count > 600) // Keep reasonable size
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

        private void DisposeCustomResources()
        {
        }
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
        // Helper method for drawing a single waveform chart
        private void DrawWaveform(Graphics g, double[] data, Pen pen, Rectangle chartBounds, double maxOverallDuration)
        {
            if(data.Length < 2 || maxOverallDuration <= 0)
            {
                return; // Not enough data or invalid max duration to draw
            }

            // Scale X to fit the width of the chartBounds
            double x_scale = (double)chartBounds.Width / data.Length;

            // Draw the waveform
            for(int i = 0; i < data.Length - 1; i++)
            {
                double p1 = data[i];
                double p2 = data[i + 1];

                // Scale Y to fit the height of the chartBounds, relative to maxOverallDuration
                // Invert Y-axis so higher values are drawn higher on the panel (Windows Forms Y-axis is top-down)
                // Adjust Y to be relative to the chartBounds.Y (top of this specific chart's area)
                double y1 = chartBounds.Y + chartBounds.Height - (p1 / maxOverallDuration) * chartBounds.Height;
                double y2 = chartBounds.Y + chartBounds.Height - (p2 / maxOverallDuration) * chartBounds.Height;

                // Ensure y coordinates stay within bounds if values are negative or exceed max
                y1 = Math.Max(chartBounds.Y, Math.Min(chartBounds.Y + chartBounds.Height, y1));
                y2 = Math.Max(chartBounds.Y, Math.Min(chartBounds.Y + chartBounds.Height, y2));

                

                double x1 = chartBounds.X + i * x_scale;
                double x2 = chartBounds.X + (i + 1) * x_scale;

                g.DrawLine(pen, (int)(chartBounds.X + x1), (int)y1, (int)(chartBounds.X + x2), (int)y2);
            }
        }

        private void DebugWaveChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.LightGray);
            AssertUtil.NotNull(DrawContext);
            Font font = DrawContext.DrawCache.GetFont("Arial", 8);

            if(DrawContext.DebugSettings.Get(DebugSetting.ShowFPS))
            {
                // Draw FPS counter
                g.DrawString($"FPS: {_controller.Fps}", font, Brushes.Black, new PointF(10, 10));
            }

            // Safely get snapshots of the data from both threads
            double[] tickData;
            double[] paintData;
            lock(_controller.TickDurationsLock) // Use the lock object from the controller
            {
                tickData = _controller.DebugTickDurations.ToArray();
            }
            lock(PaintDurationsLock) // Use the lock object from the GameView
            {
                paintData = DebugPaintDurations.ToArray();
            }
            // Disable composites for now. Not really nessesary.

            // Ensure data arrays have the same length for sum/difference calculations
            // Use the smaller length if they happen to be different (shouldn't if collected consistently)
            //int dataLength = Math.Min(tickData.Length, paintData.Length);
            //
            //double[] sumData = new double[dataLength];
            //double[] differenceData = new double[dataLength];
            //
            //for(int i = 0; i < dataLength; i++)
            //{
            //    sumData[i] = tickData[i] + paintData[i];
            //    differenceData[i] = tickData[i] - paintData[i];
            //}

            // Calculate the overall maximum duration for consistent vertical scaling across all charts
            // This considers the max of all positive values across all datasets, including absolute differences
            double maxOverallDuration = 0f;
            if(tickData.Length > 0) maxOverallDuration = Math.Max(maxOverallDuration, tickData.Max());
            if(paintData.Length > 0) maxOverallDuration = Math.Max(maxOverallDuration, paintData.Max());
            //if(sumData.Length > 0) maxOverallDuration = Math.Max(maxOverallDuration, sumData.Max());
            //if(differenceData.Length > 0) maxOverallDuration = Math.Max(maxOverallDuration, differenceData.Max(d => Math.Abs(d))); // Max absolute difference

            // Ensure maxOverallDuration is not zero to avoid division by zero
            if(maxOverallDuration == 0) maxOverallDuration = GameConstants.TICK_DURATION_MS * 2; // Default if no data or all zeros

            int panelWidth = e.ClipRectangle.Width;
            int panelHeight = e.ClipRectangle.Height;
            int chartHeight = panelHeight / 5; // Divide panel into 5 vertical sections

            // Define chart bounds and draw each waveform
            // Chart 1: Game Tick Durations (Blue)
            Pen tickPen = DrawContext.DrawCache.GetPen(Color.Blue);

            Rectangle tickChartBounds = new Rectangle(0, 1 * chartHeight, panelWidth, chartHeight);
            DrawWaveform(g, tickData, tickPen, tickChartBounds, maxOverallDuration);
            g.DrawString("Game Tick (ms)", font, Brushes.Black, new PointF(tickChartBounds.X + 5, tickChartBounds.Y + 5));


            // Chart 2: Paint Durations (Red)
            Pen paintPen = DrawContext.DrawCache.GetPen(Color.Red);
            Rectangle paintChartBounds = new Rectangle(0, 2 * chartHeight, panelWidth, chartHeight);
            DrawWaveform(g, paintData, paintPen, paintChartBounds, maxOverallDuration);
            g.DrawString("Paint (ms)", font, Brushes.Black, new PointF(paintChartBounds.X + 5, paintChartBounds.Y + 5));


            //// Chart 3: Sum (Tick + Paint) Durations (Green)
            //Pen sumPen = DrawContext.DrawCache.GetPen(Color.Green);
            //Rectangle sumChartBounds = new Rectangle(0, 3 * chartHeight, panelWidth, chartHeight);
            //DrawWaveform(g, sumData, sumPen, sumChartBounds, maxOverallDuration);
            //g.DrawString("Sum (ms)", font, Brushes.Black, new PointF(sumChartBounds.X + 5, sumChartBounds.Y + 5));
            //
            //
            //// Chart 4: Difference (Tick - Paint) Durations (Orange)
            //Pen diffPen = DrawContext.DrawCache.GetPen(Color.Orange);
            //Rectangle diffChartBounds = new Rectangle(0, 4 * chartHeight, panelWidth, chartHeight);
            //DrawWaveform(g, differenceData, diffPen, diffChartBounds, maxOverallDuration);
            //g.DrawString("Difference (ms)", font, Brushes.Black, new PointF(diffChartBounds.X + 5, diffChartBounds.Y + 5));

        }


    }
}
