﻿using System.Diagnostics;

using AssertUtils;

using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Core
{
    public class GameController
    {
        private bool _threadOpen = false; // For sanity, til I am certain I know what Im doing here.

        public enum GameState
        {
            Uninitialized,
            Playing,
            Paused,
            GameOver,
            Victory
        }

        public GameState CurrentState;

        public GameBoard Board
        {
            get
            {
                AssertUtil.NotNull(_board);
                return _board;
            }
            private set => _board = value;
        }
        private GameBoard? _board;
        public GameView View
        {
            get
            {
                AssertUtil.NotNull(_view);
                return _view;
            }
            private set => _view = value;
        }
        private GameView? _view;

        public float CurrentTime => _board?.CurrentTime ?? float.NegativeInfinity;
        public InputManager? Input => View.Input;

        private readonly System.Timers.Timer _gameLoopTimer;
        private readonly Stopwatch _stopwatch = new();

        public float GlobalTime { get; private set; } = 0f;

        private float _gameOverTime = float.NegativeInfinity;

        private bool _paused;

        private readonly float _tickDuration_ms = GameConstants.TICK_DURATION_MS; // Convert to seconds

        public readonly object TickDurationsLock = new object();
        public Queue<double> DebugTickDurations = new Queue<double>(600); // For debugging purposes, to see how long each tick takes.

        private DateTime _lastTime; // marks the beginning the measurement began
        private int _framesRendered; // an increasing count
        public int Fps { get; private set; } // the FPS calculated from the last measurement


        public GameController()
        {
            // Setup game loop timer
            _gameLoopTimer = new System.Timers.Timer() { Interval = _tickDuration_ms, AutoReset = false };
            _gameLoopTimer.Elapsed += GameLoopTimer_Tick;
        }

        public void StartTimer()
        {
            _stopwatch.Restart();
            _gameLoopTimer.Start();

            lock(TickDurationsLock)
            {
                DebugTickDurations.Clear();
            }
        }
        public void OnVictory()
        {
            // handle game over
            CurrentState = GameState.Victory;
            _gameOverTime = GlobalTime;
        }

        internal void OnDefeat()
        {
            // handle Victory
            CurrentState = GameState.GameOver;
            _gameOverTime = GlobalTime;
        }
        private void GameLoopTimer_Tick(object? sender, EventArgs e)
        {
            AssertUtil.Assert(!_threadOpen, "Game loop timer tick called while thread is open.");
            _threadOpen = true; // Set to true to prevent re-entrance.

            _framesRendered++;

            if((DateTime.Now - _lastTime).TotalSeconds >= 1)
            {
                // one second has elapsed 

                Fps = _framesRendered;
                _framesRendered = 0;
                _lastTime = DateTime.Now;
            }

            float SimDeltaTime;
            // for the gameboard's updates. Adjusted by pause and gamespeed.
            float DrawDeltaTime = GameConstants.FIXED_DELTA_TIME;
            // For drawing, which is always fixed.
            if(IsPaused())
                SimDeltaTime = 0f;
            else
                SimDeltaTime = GameConstants.FIXED_DELTA_TIME;

            GlobalTime += SimDeltaTime;

            if(CurrentState == GameState.Playing)
            {
                if(View != null && Board != null)
                {
                    Board.Update(SimDeltaTime);
                    View.Update(DrawDeltaTime); // May need to pass both in later. 
                }
            }
            else if(CurrentState == GameState.GameOver || CurrentState == GameState.Victory)
            {
                if(TimeUtils.HasElapsed(GlobalTime, _gameOverTime, GameConstants.GAME_RESTART_DELAY))
                {
                    // Game crashes once the game starts and update happens, because CreateGameBoard isn't sufficient.
                    // This needs to be sent to the actual Program.cs, since that is where all the initialization happens.
                    // Dunno how to do that atm.
                    View?.Dispose();
                    //CreateGameBoard();
                    //StartGame();
                }
            }
            Input?.EndFrame();

            // Restart the timer for the next tick, using elapsed time for accuracy
            double elapsed = _stopwatch.Elapsed.TotalMilliseconds;

            lock(TickDurationsLock)
            {
                DebugTickDurations.Enqueue(elapsed);
                // Keep the queue from growing indefinitely
                if(DebugTickDurations.Count > 60) // Max 10 seconds of data at 60 FPS
                {
                    DebugTickDurations.Dequeue();
                }
            }

            double delay = Math.Max(0.1, GameConstants.TICK_DURATION_MS - elapsed);

            _stopwatch.Restart();
            _gameLoopTimer.Interval = delay;
            _threadOpen = false; // Set to false to allow continuation
            _gameLoopTimer.Start();
        }

        public GameBoard CreateGameBoard()
        {
            return Board = new GameBoard(this);
        }
        public GameView CreateGameView()
        {
            AssertUtil.NotNull(Board);
            return View = new GameView(this, Board);

        }

        internal void StartGame()
        {
            if(Board != null)
            {
                CurrentState = GameState.Playing;
            }
        }

        internal void TogglePause()
        {
            _paused = !_paused;
        }


        internal bool IsPaused()
        {
            AssertUtil.NotNull(View);
            return _paused || View.DebugPanelVisible; // Manual pause, or debug menu open.
        }
    }
}