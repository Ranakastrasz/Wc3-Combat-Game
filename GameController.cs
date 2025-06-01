using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Util;
using Timer = System.Windows.Forms.Timer;


namespace Wc3_Combat_Game
{
    internal class GameController
    {

        public enum GameState
        {
            Uninitialized,
            Playing,
            Paused,
            GameOver
        }

        public GameState CurrentState;

        public GameSession? Session { get; set; }
        public readonly GameView View;


        public float CurrentTime => Session?.CurrentTime ?? float.NegativeInfinity;
        public InputManager Input => View.Input;

        private readonly Timer _gameLoopTimer;


        public float GlobalTime { get; private set; } = 0f;

        private float _gameOverTime = float.NegativeInfinity;

        public GameController()
        {
            View = new(this);
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
                if (View != null && Session != null)
                {
                    Session.Update(deltaTime);
                    View.Update(deltaTime);
                }
            }
            else if (CurrentState == GameState.GameOver)
            {
                if (GlobalTime >= _gameOverTime + GameConstants.GAME_RESTART_DELAY)
                {
                    CreateSession();
                    SyncDrawables();
                    StartGame();
                }
            }
            Input.EndFrame();

        }

        public void CreateSession()
        {
            Session = new GameSession(this);
        }

        internal void StartGame()
        {
            if (Session != null)
            {
                CurrentState = GameState.Playing;
            }
        }

        internal void SyncDrawables()
        {
            if (View != null && Session != null)
            {
                View?.SetDrawables(Session.Projectiles, Session.Units);

            }

        }
    }
}
