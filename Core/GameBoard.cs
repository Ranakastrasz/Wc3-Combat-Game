using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Effects;
using static Wc3_Combat_Game.Core.GameConstants;
using Wc3_Combat_Game.Interface.Controllers;
using Wc3_Combat_Game.Terrain;
using System.Data;
using MathUtils;
using Wc3_Combat_Game.Waves;

namespace Wc3_Combat_Game.Core
{
    public class GameBoard : IBoardContext, IDrawContext
    {
        private readonly GameController? _controller;

        public float CurrentTime { get; private set; } = 0f;

        //private List<PrototypeUnit> _waveUnits = new();
        //private List<int> _waveUnitCounts = new();

        private List<Wave> _waves = new();
        private int _waveCurrent;
        private int _waveSpawnsRemaining;

        private List<Vector2> spawnPoints;


        // Entities.
        public Unit? PlayerUnit { get; private set; }

        internal EntityManager<Projectile> Projectiles { get; private set; } = new();
        internal EntityManager<Unit> Units { get; private set; } = new();

        public EntityManager<Entities.Entity> Entities { get; private set; } = new();


        public Map Map { get; private set; }
        public float TileSize { get; private set; }

        private float _lastEnemySpawned = 0f;


        public GameBoard()
        {

            // Parse the map and setup spawn points
            TileSize = 32f;

            Map = Map.ParseMap(Map.map1, TileSize);
            List<Vector2Int> portals = Map.GetTilesMatching('P');
            spawnPoints = portals.Select(p => (p.ToVector2() + new Vector2(0.5f, 0.5f)) * TileSize).ToList();

            // Setup wave units and counts
            InitWaves();

            // PlayerUnit remains uninitialized here (depends on _controller.Input)
        }

        // Optional constructor that sets the controller and initializes dependent things
        public GameBoard(GameController? controller) : this()
        {
            _controller = controller;
        }

        public void InitWaves()
        {
            // Will be in wave class when we get there.
            _waveCurrent = -1;
            _waveSpawnsRemaining = 0;

            var meleeWeaponBase = new PrototypeWeaponBasic(new Effects.Action(), 1f, 20f);
            var weapon5Damage = meleeWeaponBase.SetDamage(5f);
            var weapon10Damage = meleeWeaponBase.SetDamage(10f);
            var weapon25Damage = meleeWeaponBase.SetDamage(25f);
            var weapon200Damage = meleeWeaponBase.SetDamage(200f);

            var rangedWeaponBase = new PrototypeWeaponBasic(
                new ActionProjectile(new PrototypeProjectile(5, 225f, 4f, null, Color.Cyan)),
                1f,
                150f);

            var weapon10DamageRanged = rangedWeaponBase.SetDamage(10f);

            _waves.Add(new Wave(new PrototypeUnit(weapon5Damage       , 10f,   2f,  8f,  75f, Color.Brown  , PrototypeUnit.DrawShape.Circle), 16));
            _waves.Add(new Wave(new PrototypeUnit(weapon10Damage      , 20f, 0.1f, 12f, 100f, Color.Red    , PrototypeUnit.DrawShape.Circle), 16));
            _waves.Add(new Wave(new PrototypeUnit(weapon10DamageRanged, 30f, 0.1f, 10f,  50f, Color.Orange , PrototypeUnit.DrawShape.Square), 8));
            _waves.Add(new Wave(new PrototypeUnit(weapon25Damage      , 80f,   2f, 20f,  75f, Color.Red    , PrototypeUnit.DrawShape.Square), 4));
            _waves.Add(new Wave(new PrototypeUnit(weapon200Damage     , 400f,  0f, 30f, 125f, Color.DarkRed, PrototypeUnit.DrawShape.Square), 1));

        }

        public void InitPlayer()
        {
            AssertUtil.AssertNotNull(_controller);
            AssertUtil.AssertNotNull(_controller.Input);

            PrototypeWeaponBasic weapon = new PrototypeWeaponBasic(new ActionProjectile(new PrototypeProjectile(5f,
                600f,
                2f,
                new ActionDamage(10f),
                Color.Blue)),
                0.20f,
                float.PositiveInfinity);

            PrototypeUnit playerUnit = new((PrototypeWeapon)weapon, 100f, 0.1f, 10f, 150f, Color.Green, PrototypeUnit.DrawShape.Circle);

            PlayerUnit = UnitFactory.SpawnUnit(playerUnit, (Vector2)GAME_BOUNDS.Center(), new IPlayerController(_controller.Input), TeamType.Ally);

            AddUnit(PlayerUnit);
        }




        private bool CheckGameOverCondition(IBoardContext context)
        {
            AssertUtil.AssertNotNull(PlayerUnit);
            if (PlayerUnit.IsExpired(context))
            {
                AssertUtil.AssertNotNull(_controller);
                _controller.OnVictory();
                return true;
            }
            return false;
        }
        private bool CheckVictoryCondition()
        {
            if (_waveCurrent == _waves.Count)
            {
                AssertUtil.AssertNotNull(_controller);
                _controller.OnVictory();
                return true;
            }
            return false;
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
                    Vector2 spawnPoint = spawnPoints[RandomUtils.RandomIntBelow(spawnPoints.Count)]; // Poor, but for now

                    Unit unit = UnitFactory.SpawnUnit(_waves[_waveCurrent].Unit,spawnPoint, new IBasicAIController(), TeamType.Enemy);
                    unit.Target = PlayerUnit;
                    AddUnit(unit);
                    // Elite
                    if (_waveSpawnsRemaining > 0 && _waveSpawnsRemaining == _waves[_waveCurrent].CountToSpawn)
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
                    // Should be .Count, but also needs a boss tag check. Later.
                    // After all, we do next wave when less than 1/8th remain, or less than 8, maybe. dunno. 
                    // Right now, its 100%.
                    // New Wave

                    _waveCurrent++;

                    if (!CheckVictoryCondition())
                    {
                        _waveSpawnsRemaining = _waves[_waveCurrent].CountToSpawn;
                    }
                }


            }

            // Update Entities.

            Units.UpdateAll(deltaTime, this);
            Projectiles.UpdateAll(deltaTime, this);
            // Seperate required, because units can create projectiles.
            // Admittedly, eventually projectiles will be able to create projectiles, Starburst or Delayed cast.
            // so a method to do this right is needed.

            CheckCollision(deltaTime);


            // Cleanup dead entities.
            Projectiles.RemoveExpired(this);
            Units.RemoveExpired(this);
            Entities.RemoveExpired(this);

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
            AssertUtil.AssertNotNull(PlayerUnit);
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
            Entities.Add(p);
        }

        public void AddUnit(Unit u)
        {
            Units.Add(u);
            Entities.Add(u);
        }

    }
}
