
using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Controllers;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
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

            var meleeWeaponBase = new TargetedAbilityPrototype(null, 1f, 20f);
            var weapon5Damage = meleeWeaponBase.WithDamage(5f);
            var weapon10Damage = meleeWeaponBase.WithDamage(10f);
            var weapon25Damage = meleeWeaponBase.WithDamage(25f);
            var weapon200Damage = meleeWeaponBase.WithDamage(200f);

            var rangedWeaponBase = new TargetedAbilityPrototype(
                new ProjectileAction(new ProjectilePrototype("Ranged Weapon",2.5f, 225f, 4f, null, Color.DarkMagenta)),
                0.5f,
                150f,10f);

            //var snareProjectile = 
            //
            var rangedWeaponSnare = new TargetedAbilityPrototype(
                new ProjectileAction(new ProjectilePrototype("Snare Projectile",2.5f, 225, 16f,
                    new SlowAction(0.5f), Color.Cyan)),
                0.5f,
                150f,5f);


            var unit = new UnitPrototype("Basic",15f, 2f, 4f, 50f, Color.Brown, 6);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(5f));
            //_waves.Add(new Wave(unit, 32));
            //
            //unit = new UnitPrototype("Blitz",10f, 0.0f, 4f, 75f, Color.DarkGoldenrod, 3);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(10f));
            //_waves.Add(new Wave(unit, 32));
            //
            //unit = new UnitPrototype("Blaster",30f, 0.0f, 5f, 40f, Color.Orange, 5);
            //unit = unit.AddWeapon(rangedWeaponBase.WithDamage(10f));
            //_waves.Add(new Wave(unit, 16));
            //
            //unit = new UnitPrototype("Brute",80f, 2f, 10f, 50f, Color.Brown, 6);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(25f));
            //_waves.Add(new Wave(unit, 8));
            
            unit = new UnitPrototype("Boss",400f, 0f, 15f, 100f, Color.DarkRed, 4);
            unit = unit.AddWeapon(meleeWeaponBase.WithDamage(90f));
            unit = unit.AddWeapon(rangedWeaponSnare);
            _waves.Add(new Wave(unit, 1));

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
                    Vector2 spawnPoint = _spawnPoints[RandomUtils.RandomIntBelow(_spawnPoints.Count)]; // Poor, but for now
                    Unit unit;
                    if(_waveSpawnsRemaining == _waves[_currentWave].CountToSpawn && _waves[_currentWave].CountToSpawn > 1)
                    {
                        UnitPrototype elitePrototype = _waves[_currentWave].Unit;
                        elitePrototype = elitePrototype.WithLife(elitePrototype.MaxLife * 4, elitePrototype.LifeRegen * 4);
                        elitePrototype = elitePrototype.WithSpeed(elitePrototype.Speed * 1.2f);
                        elitePrototype = elitePrototype.WithRadius(elitePrototype.Radius * 1.5f);
                        //elitePrototype = elitePrototype.WithColors(Color.Black, elitePrototype.DamagedColor, elitePrototype.DeadColor, elitePrototype.PolygonCount);
                        AbilityPrototype[] abilities = new AbilityPrototype[elitePrototype.Abilities.Length];
                        for(int x = 0; x < elitePrototype.Abilities.Length; x++)
                        {
                            AbilityPrototype ability = elitePrototype.Abilities[x];
                            // hacky solution.
                            if(ability is TargetedAbilityPrototype targetedAbility)
                            {
                                float damage =  targetedAbility.GetDamage();
                                targetedAbility = targetedAbility.WithDamage(damage * 4);
                                abilities[x] = targetedAbility;
                            }
                        }
                        elitePrototype = elitePrototype.WithAbilities(abilities);
                        unit = UnitFactory.SpawnUnit(elitePrototype, spawnPoint, new UnitControllerCore(), Team.Enemy, context);
                    }
                    else
                    {
                        unit = UnitFactory.SpawnUnit(_waves[_currentWave].Unit, spawnPoint, new UnitControllerCore(), Team.Enemy, context);

                    }

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
