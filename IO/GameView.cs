using System.Drawing;
using System.Net.NetworkInformation;
using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;
using AssertUtils;
using static Wc3_Combat_Game.Core.GameController;

namespace Wc3_Combat_Game
{

    public partial class GameView : Form
    {
        internal readonly GameController _controller;

        private Camera _camera;


        private IDrawContext? _drawContext;

        private Font? _gridFont;
#region InputHooks
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
                Input.OnMouseDown(ScreenToWorld(e.Location));
        }

        private void MainGameWindow_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
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
            _drawContext = context;

            this.ClientSize = GameConstants.CAMERA_BOUNDS.Size.ToSize();
            this._camera = new Camera();
            _camera.LerpFactor = 10f;

            _camera.Zoom = 3;
            _camera.Width = GameConstants.CAMERA_BOUNDS.Width;
            _camera.Height = GameConstants.CAMERA_BOUNDS.Height;
            //AssertUtil.AssertNotNull(_drawContext.PlayerUnit);
            //_camera.FollowUnit(_drawContext.PlayerUnit!);


            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Form gets key events even if controls are focused
            this.Focus();            // Ensure the form has focus


            this.KeyDown += MainGameWindow_KeyDown;
            this.KeyUp += MainGameWindow_KeyUp;

            this.MouseDown += MainGameWindow_MouseDown;
            this.MouseUp += MainGameWindow_MouseUp;
            this.MouseMove += MainGameWindow_MouseMove;

            this.Resize +=  OnWindowSizeChanged;

            _gridFont = null;
        }

        public void OnWindowSizeChanged(object? sender, EventArgs e)
        {
            _camera.Width = ClientSize.Width;
            _camera.Height = ClientSize.Height;
        }
        public void Update(float deltaTime)
        {
            _camera.Update(deltaTime);
            

            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);


            // Apply camera transform
            using var transform = _camera.GetTransform();
            g.Transform = transform;


            // Draw Map.
            if (_drawContext != null)
            {
                Map? map = _drawContext.Map;
                AssertUtil.AssertNotNull(map);

                //_gridFont ??= FontUtils.FitFontToTile(g, "Consolas", 32f, FontStyle.Regular, GraphicsUnit.Pixel);//new Font("Consolas", 32f, FontStyle.Regular, GraphicsUnit.Pixel);


                float tileSize = map.TileSize;

                for (int y = 0; y < map.TileMap.GetLength(1); y++)
                {
                    for (int x = 0; x < map.TileMap.GetLength(0); x++)
                    {
                        Tile tile = map.TileMap[x, y];
                        Brush brush = new SolidBrush(tile.GetColor); // optionally cache these

                        //g.FillRectangle(brush,x*tileSize,y*tileSize,tileSize,tileSize);
                        // Tell tiles to draw themselves.
                        tile.Draw(g,tileSize);

                        brush.Dispose(); // only if not cached
                    }
                }

            }

#if DEBUG
            g.DrawRectangle(Pens.White, GameConstants.GAME_BOUNDS);
            g.DrawRectangle(Pens.Blue, GameConstants.SPAWN_BOUNDS);
#endif
            _drawContext?.Entities?.ForEach(p => p.Draw(g, _drawContext));


            if (_controller.CurrentState == GameState.GameOver || _controller.CurrentState == GameState.Victory)
            {
                g.ResetTransform();

                using var gameOverFont = new Font("Arial", 24, FontStyle.Bold);
                string message = _controller.CurrentState == GameState.GameOver ? "Game Over" : "Victory!";
                SizeF textSize = g.MeasureString(message, gameOverFont);

                float x = (ClientSize.Width - textSize.Width) / 2;
                float y = (ClientSize.Height - textSize.Height) / 2;

                g.DrawString(message, gameOverFont, Brushes.White, x, y);


            }
        }

        public void RegisterPlayer(Unit playerUnit)
        {
            _camera.FollowUnit(playerUnit);
            _camera.SnapToUnit(playerUnit);
        }

        private void DisposeCustomResources() => _gridFont?.Dispose();
    }
}
