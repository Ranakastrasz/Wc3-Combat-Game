using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;
using Timer = System.Windows.Forms.Timer;


namespace Wc3_Combat_Game.Core
{
    public class GameController
    {

        public enum GameState
        {
            Uninitialized,
            Playing,
            Paused,
            GameOver,
            Victory
        }

        public GameState CurrentState;

        public GameBoard? Board { get; set; }
        public GameView? View { get; set; }



        public float CurrentTime => Board?.CurrentTime ?? float.NegativeInfinity;
        public InputManager ?Input => View?.Input;

        private readonly Timer _gameLoopTimer;


        public float GlobalTime { get; private set; } = 0f;

        private float _gameOverTime = float.NegativeInfinity;

        public GameController()
        {
            // Setup game loop timer
            _gameLoopTimer = new() { Interval = GameConstants.TICK_DURATION_MS };
            _gameLoopTimer.Tick += GameLoopTimer_Tick;
        }

        public void StartTimer()
        {
            _gameLoopTimer.Start();
        }
        public void OnGameOver()
        {
            // handle game over
            CurrentState = GameState.GameOver;
            _gameOverTime = GlobalTime;
        }

        private void GameLoopTimer_Tick(object? sender, EventArgs e)
        {
            float deltaTime = GameConstants.FIXED_DELTA_TIME;
            GlobalTime += deltaTime;

            if (CurrentState == GameState.Playing)
            {
                if (View != null && Board != null)
                {
                    Board.Update(deltaTime);
                    View.Update(deltaTime);
                }
            }
            else if (CurrentState == GameState.GameOver)
            {
                if (TimeUtils.HasElapsed(GlobalTime, _gameOverTime, GameConstants.GAME_RESTART_DELAY))
                {
                    CreateGameBoard();
                    StartGame();
                }
            }
            Input?.EndFrame();

        }

        public GameBoard CreateGameBoard()
        {
            return Board = new GameBoard(this);
        }
        public GameView CreateGameView()
        {
            AssertUtil.Assert(() => Board != null);
            return View = new GameView(this, Board);
            
        }

        internal void StartGame()
        {
            if (Board != null)
            {
                CurrentState = GameState.Playing;
            }
        }

    }
}
