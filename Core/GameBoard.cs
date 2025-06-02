using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Effects;
using static Wc3_Combat_Game.Core.GameConstants;
using Wc3_Combat_Game.Interface.Weapons;
using Wc3_Combat_Game.Interface.Controllers;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Core
{
    public class GameBoard : IBoardContext, IDrawContext
    {
        private readonly GameController _controller;

        public float CurrentTime { get; private set; } = 0f;

        private List<PrototypeUnit> _waveUnits = new();
        private List<int> _waveUnitCounts = new();
        private int _waveCurrent;
        private int _waveSpawnsRemaining;


        // Entities.
        public Unit PlayerUnit { get; private set; }

        internal EntityManager<Projectile> Projectiles { get; private set; } = new();
        internal EntityManager<Unit> Units { get; private set; } = new();

        public Tile[,] TileGrid { get; private set; }

        private float _lastEnemySpawned = 0f;


        public GameBoard(GameController controller)
        {
            _controller = controller;


            // Init player
            PrototypeWeaponBasic weapon = new PrototypeWeaponBasic(new EffectProjectile(new PrototypeProjectile(5f,
            600f,
            2f,
            new EffectDamage(10f),
            Color.Blue)),
            0.20f,
            float.PositiveInfinity);


            PrototypeUnit playerUnit = new((PrototypeWeapon)weapon, 100f, 0.1f, 10f, 150f, Color.Green, PrototypeUnit.DrawShape.Circle);

            PlayerUnit = UnitFactory.SpawnUnit(playerUnit, (Vector2)GAME_BOUNDS.Center(), new IPlayerController(controller.Input), TeamType.Ally);

            

            AddUnit(PlayerUnit);


            var meleeWeaponBase = new PrototypeWeaponBasic(new Effect(), 1f, 20f);
            var weapon5Damage = meleeWeaponBase.SetDamage(5f);
            var weapon10Damage = meleeWeaponBase.SetDamage(10f);
            var weapon25Damage = meleeWeaponBase.SetDamage(25f);
            var weapon200Damage = meleeWeaponBase.SetDamage(200f);

            var rangedWeaponBase = new PrototypeWeaponBasic(
                new EffectProjectile(new PrototypeProjectile(5, 450f, 2f, null, Color.Cyan)),
                1f,
                150f);

            var weapon10DamageRanged = rangedWeaponBase.SetDamage(10f);


            _waveUnits.Add(new(weapon5Damage,       10f, 2f  ,  10f, 75f, Color.Brown  , PrototypeUnit.DrawShape.Circle));
            _waveUnits.Add(new(weapon10Damage,      20f, 0.1f,  15f, 100f, Color.Red    , PrototypeUnit.DrawShape.Circle));
            _waveUnits.Add(new(weapon10DamageRanged,30f, 0.1f,  15f, 50f, Color.Orange , PrototypeUnit.DrawShape.Square));
            _waveUnits.Add(new(weapon25Damage,      80f, 2f  ,  25f, 750f, Color.Red    , PrototypeUnit.DrawShape.Square));
            _waveUnits.Add(new(weapon200Damage,     2000f,0f  , 50f,125f, Color.DarkRed, PrototypeUnit.DrawShape.Square));



            _waveUnitCounts.Add(64);
            _waveUnitCounts.Add(64);
            _waveUnitCounts.Add(32);
            _waveUnitCounts.Add(16);
            _waveUnitCounts.Add(1);

            _waveCurrent = -1;
            _waveSpawnsRemaining = 0;

            // ParseMap
            string[] map = Map.map1;
            TileGrid = new Tile[map[0].Length, map.Length];
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    char c = Map.map1[y][x];
                    TileGrid[x, y] = Tile.CharToTile(c);
                }
            }

        }




        private void CheckGameOverCondition(IBoardContext context)
        {
            if (PlayerUnit.IsExpired(context))
            {
                _controller.OnGameOver();
            }
        }

        public void Update(float deltaTime)
        {
            CurrentTime += deltaTime;


            if (TimeUtils.HasElapsed(CurrentTime,_lastEnemySpawned,ENEMY_SPAWN_COOLDOWN))
            {
                if (_waveSpawnsRemaining > 0)
                {
                    _lastEnemySpawned = CurrentTime;
                    _waveSpawnsRemaining--;
                    Unit unit = UnitFactory.SpawnUnit(_waveUnits[_waveCurrent], (Vector2)RandomUtils.RandomPointBorder(SPAWN_BOUNDS), new IBasicAIController(), TeamType.Enemy);
                    unit.Target = PlayerUnit;
                    AddUnit(unit);
                    // Elite
                    if (_waveSpawnsRemaining > 0 && _waveSpawnsRemaining == _waveUnitCounts[_waveCurrent])
                    {
                        // unit.MaxHealth *= 4;
                        // unit.Health *= 4;
                        // unit.Speed += 25;
                        // unit.Damage *= 4;
                        // I probably need to... Do something to register untis as unique.
                        // I can't just set these values.

                    }
                }
                else if (!Units.Entities.Any(s => s.IsAlive && s.Team == TeamType.Enemy))
                {
                    // New Wave
                    if (_waveCurrent < _waveUnits.Count)
                    {
                        _waveCurrent++;
                        _waveSpawnsRemaining = _waveUnitCounts[_waveCurrent];

                    }
                    else
                    {
                        // Loop it, XD
                        _waveCurrent = 0;
                        //CheckGameOverCondition();
                    }
                }


            }

            // Update Entities.

            PlayerUnit.Update(deltaTime, this);
            Projectiles.UpdateAll(deltaTime, this);
            Units.UpdateAll(deltaTime, this);

            CheckCollision(deltaTime);


            // Cleanup dead entities.
            Projectiles.RemoveExpired(this);
            Units.RemoveExpired(this);


            CheckGameOverCondition(this);
            // End Logic
        }

        private void CheckCollision(float deltaTime)
        {
            foreach (Projectile projectile in Projectiles.Entities.Where(p => p.IsAlive))
            {
                foreach (Unit unit in Units.Entities.Where(p => p.IsAlive && p.Team.IsHostileTo(projectile.Team)))
                {
                    if (projectile.Intersects(unit))
                    {
                        projectile.Die(this);
                        projectile.ImpactEffect?.ApplyToEntity(projectile.Caster, projectile, unit, this);

                        break;
                    }
                }
            }

            foreach (Projectile projectile in Projectiles.Entities)
            {
                if (!projectile.BoundingBox.IntersectsWith(GAME_BOUNDS))
                {
                    projectile.Die(this);
                }
            }

            if (!GAME_BOUNDS.Contains(PlayerUnit.BoundingBox))
            {
                var bounds = GAME_BOUNDS;
                var halfSize = new SizeF(PlayerUnit.Size / 2f, PlayerUnit.Size / 2f);

                PlayerUnit.Position = new Vector2(
                    Math.Clamp(PlayerUnit.Position.X, bounds.Left + halfSize.Width, bounds.Right - halfSize.Width),
                    Math.Clamp(PlayerUnit.Position.Y, bounds.Top + halfSize.Height, bounds.Bottom - halfSize.Height)
                );
            }
        }

        public void AddProjectile(Projectile p)
        {
            Projectiles.Add(p);
        }

        public void AddUnit(Unit u)
        {
            Units.Add(u);
        }

    }
}
