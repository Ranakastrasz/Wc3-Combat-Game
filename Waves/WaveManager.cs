
using System.Collections.Immutable;
using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Data;
using Wc3_Combat_Game.Entities.Components.Controllers;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Entities.Components.Prototype.Factories;
using Wc3_Combat_Game.Entities.EntityTypes;
using Wc3_Combat_Game.Util;


using static Wc3_Combat_Game.Core.GameConstants;

namespace Wc3_Combat_Game.Waves
{
    internal class WaveManager
    {
        public List<Wave> Waves { get => _waves; }
        private List<Wave> _waves = new();
        private List<Vector2>? _spawnPoints;

        private int _currentWave;
        private int _waveSpawnsRemaining;
        private float _lastEnemySpawned = float.NegativeInfinity;

        public bool _wavesPaused = false;
        public bool AllWavesComplete { get => _currentWave >= _waves.Count; }

        public int CurrentWave { get => _currentWave; private set => _currentWave = value; }
        public int WaveSpawnsRemaining { get => _waveSpawnsRemaining; private set => _waveSpawnsRemaining = value; }

        private enum State
        {
            UnInitialized,
            Waiting,
            InProgress,
            Completed,
            Paused
        }
        //State _state = State.UnInitialized;

        public WaveManager()
        {
            _currentWave = -1;
            _waveSpawnsRemaining = 0;
        }

        public void InitWaves(List<Vector2> spawnPoints)
        {
            AssertUtil.NotNull(spawnPoints);
            _spawnPoints = spawnPoints;

            WaveBuilder.BuildWaves(_waves);
        }
        public void Update(float deltaTime, IBoardContext context)
        {
            if(_waves.Count == 0) return;
            if(_wavesPaused) return;
            if(_spawnPoints == null) return;
            if(_spawnPoints.Count == 0) return; // No spawn points, no spawning.

            if(_currentWave < 0)
            {
                _currentWave = 0;
                _waveSpawnsRemaining = _waves[_currentWave].CountToSpawn;
            }
            if(context.GetEnemyUnits(Team.Enemy).Count <= 0 && _waveSpawnsRemaining == 0)
            {
                // eventually, we do next wave when less than 1/8th remain, or less than 8, maybe. dunno. 
                // Right now, its 100%.
                // New Wave

                _currentWave++;

                if(_currentWave < _waves.Count)
                {
                    _waveSpawnsRemaining = _waves[_currentWave].CountToSpawn;
                }
                else
                {
                    // No more waves, stop spawning.
                    _waveSpawnsRemaining = 0;
                    _wavesPaused = true;
                    return;
                }
            }

            float CurrentTime = context.CurrentTime;

            if(TimeUtils.HasElapsed(CurrentTime, _lastEnemySpawned, ENEMY_SPAWN_COOLDOWN))
            {
                if(_waveSpawnsRemaining > 0)
                {
                    Vector2 spawnPoint = _spawnPoints[RandomUtils.RandomIntBelow(_spawnPoints.Count)];
                    UnitPrototype unitPrototype = _waves[_currentWave].Unit;

                    // Check for the last unit of the wave.
                    if(_waveSpawnsRemaining == _waves[_currentWave].CountToSpawn && _waves[_currentWave].CountToSpawn > 1)
                    {
                        unitPrototype = UnitFactory.CreateEliteUnit(unitPrototype);
                    }

                    Unit unit = Unit.SpawnUnit(unitPrototype, spawnPoint, new UnitControllerCore(), Team.Enemy, context);

                    //unit.TargetUnit = PlayerUnit;
                    context.AddUnit(unit);
                    _lastEnemySpawned = CurrentTime;
                    _waveSpawnsRemaining--;

                }
            }
        }


        // Might use, might not. Dont know if it is the right structure yet.
        //public void StartWaves()
        //{
        //    if(_waves.Count == 0) return;
        //    _wavesPaused = false;
        //    _currentWave = 0;
        //    _waveSpawnsRemaining = _waves[_currentWave].CountToSpawn;
        //    _lastEnemySpawned = float.NegativeInfinity; // Reset spawn timer.
        //}
        //public void NextWave()
        //{
        //    if(_waves.Count == 0) return;
        //    if(_currentWave >= _waves.Count - 1) return; // No more waves to go to.
        //    _currentWave++;
        //    _waveSpawnsRemaining = _waves[_currentWave].CountToSpawn;
        //    _lastEnemySpawned = float.NegativeInfinity; // Reset spawn timer.
        //}
        //public void PauseWaves(bool pause)
        //{
        //    _wavesPaused = pause;
        //}
        //
        //public bool IsWaveOver()
        //{
        //    return _waves.Count > 0 && _currentWave >= 0 && _currentWave < _waves.Count &&
        //           _waveSpawnsRemaining <= 0 && _waves[_currentWave].CountToSpawn > 0;
        //}
    }
}
