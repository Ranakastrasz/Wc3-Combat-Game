using AssertUtils;
using AStar;
using AStar.Options;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Wc3_Combat_Game.Components.Actions;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Components.Controllers;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Prototype.Weapons;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Waves;
using static Wc3_Combat_Game.Core.GameConstants;
//using Wc3_Combat_Game.IO.Load.GameSchema;

namespace Wc3_Combat_Game.Core
{
    public class GameBoard : IBoardContext, IDrawContext, IDisposable
    {
        private readonly GameController? _controller;

        public float CurrentTime { get; private set; } = 0f;

        //private List<PrototypeUnit> _waveUnits = new();
        //private List<int> _waveUnitCounts = new();

        private List<Wave> _waves = new();
        private int _waveCurrent;
        private int _waveSpawnsRemaining;

        private List<Vector2> spawnPoints = new();

        Camera? IDrawContext.Camera => _controller?.View?.Camera;

        // Entities.
        public Unit? PlayerUnit { get; private set; }

        internal EntityManager<Projectile> Projectiles { get; private set; } = new();
        internal EntityManager<Unit> Units { get; private set; } = new();

        public EntityManager<Entities.Entity> Entities { get; private set; } = new();

        private float _lastCacheUpdateTime = float.NegativeInfinity;
        private Dictionary<Team, List<Unit>> _friendlyUnitsCache = new Dictionary<Team, List<Unit>>();
        private Dictionary<Team, List<Unit>> _enemyUnitsCache = new Dictionary<Team, List<Unit>>();

        public IReadOnlyList<Unit> GetFriendlyUnits(Team team)
        {
            if(_lastCacheUpdateTime < CurrentTime) // Every tick.
                UpdateCaches();
            if(_friendlyUnitsCache.TryGetValue(team, out List<Unit>? list))
                return list;
            return Array.Empty<Unit>(); // Return an empty array if team not found (no units for it)

        }

        public IReadOnlyList<Unit> GetEnemyUnits(Team team)
        {
            if(_lastCacheUpdateTime < CurrentTime)
                UpdateCaches();
            if(_enemyUnitsCache.TryGetValue(team, out List<Unit>? list))
                return list;
            return Array.Empty<Unit>();
        }
        private void UpdateCaches()
        {
            _lastCacheUpdateTime = CurrentTime;

            // Step 1: Ensure all possible teams have their lists initialized
            foreach(Team team in Enum.GetValues<Team>())
            {
                if(!_friendlyUnitsCache.TryGetValue(team, out List<Unit>? friendlyUnits))
                {
                    _friendlyUnitsCache[team] = new List<Unit>();
                }
                else
                {
                    friendlyUnits.Clear(); // Clear for reuse
                }

                if(!_enemyUnitsCache.TryGetValue(team, out List<Unit>? enemyUnits))
                {
                    _enemyUnitsCache[team] = new List<Unit>();
                }
                else
                {
                    _enemyUnitsCache[team].Clear(); // Clear for reuse
                }
            }

            // Step 2: Populate the caches based on unit relationships
            foreach(var currentUnit in Units.Entities.Where(u => u.IsAlive))
            {
                foreach(var otherUnit in Units.Entities.Where(u => u.IsAlive))
                {
                    if(currentUnit == otherUnit)
                    {
                        // A unit is always friendly to itself.
                        _friendlyUnitsCache[currentUnit.Team].Add(otherUnit);
                    }
                    else if(currentUnit.Team.IsFriendlyTo(otherUnit.Team))
                    {
                        // currentUnit considers otherUnit friendly, so add otherUnit to currentUnit's friendly list
                        _friendlyUnitsCache[currentUnit.Team].Add(otherUnit);
                    }
                    else if(currentUnit.Team.IsHostileTo(otherUnit.Team))
                    {
                        // currentUnit considers otherUnit hostile, so add otherUnit to currentUnit's enemy list
                        _enemyUnitsCache[currentUnit.Team].Add(otherUnit);
                    }
                }
            }
        }

        //
        //IEnumerable<Unit> entities = context.Entities.Entities
        //            .OfType<Unit>()
        //            .Where(u => u.IsAlive && u.Team.IsFriendlyTo(unit.Team));

        public Map? Map { get; private set; }
        public PathFinder? PathFinder { get; private set; }
        public float TileSize { get; private set; }

        public DebugSettings DebugSettings = new DebugSettings(); // May or may not stay here.
        DebugSettings IDrawContext.DebugSettings => DebugSettings;
        
        public DrawCache DrawCache { get; private set; }

        private float _lastEnemySpawned = 0f;


        public GameBoard()
        {
            Map = null;
            PathFinder = null;
            DrawCache = new DrawCache();
        }


        // Optional constructor that sets the controller and initializes dependent things
        public GameBoard(GameController? controller) : this()
        {
            _controller = controller;
        }

        public void InitMap(string[] mapString, float tileSize = 32f)
        {
            TileSize = tileSize;

            Map = Map.ParseMap(mapString, TileSize);
            List<Point> portals = Map.GetTilesMatching('P');
            spawnPoints = portals.Select(p => (p.ToVector2() + new Vector2(0.5f, 0.5f)) * TileSize).ToList();

            PathFinderOptions options = new PathFinderOptions
            {
                UseDiagonals = false
            };
            PathFinder = new PathFinder(Map.PathfinderGrid.Grid, options);

        }

        public void InitWaves()
        {
            // Will be in wave class when we get there.
            _waveCurrent = -1;
            _waveSpawnsRemaining = 0;

            var meleeWeaponBase = new WeaponPrototypeBasic(new GameplayActionNull(), 1f, 20f);
            var weapon5Damage = meleeWeaponBase.SetDamage(5f);
            var weapon10Damage = meleeWeaponBase.SetDamage(10f);
            var weapon25Damage = meleeWeaponBase.SetDamage(25f);
            var weapon200Damage = meleeWeaponBase.SetDamage(200f);

            var weapon10DamageRanged = new WeaponPrototypeBasic(
                new ProjectileAction(new ProjectilePrototype(2.5f, 225f, 4f, new DamageAction(10f), Color.DarkMagenta)),
                0.5f,
                150f,10f); // For some reason, this doesnt end up using mana. Dunno why.

            var weapon10DamageRangedRapid = new WeaponPrototypeBasic(
                new ProjectileAction(new ProjectilePrototype(5f, 375f, 4f, new DamageAction(10f), Color.Black)),
                0.25f,
                500f);


            var wave3Unit = new UnitPrototype(weapon10DamageRanged, 30f,   0f, 10f,0.5f, 5f,  40f, Color.Orange , UnitPrototype.DrawShape.Square);

            //_waves.Add(new Wave(new UnitPrototype(weapon5Damage       , 10f,   2f,  8f,  75f, Color.Brown  , UnitPrototype.DrawShape.Circle), 1));
            _waves.Add(new Wave(new UnitPrototype(weapon5Damage       , 12f,   2f,  4f,  50f, Color.Brown  , UnitPrototype.DrawShape.Circle), 32));
            _waves.Add(new Wave(new UnitPrototype(weapon10Damage      , 10f, 0.1f,  4f,  75f, Color.Pink   , UnitPrototype.DrawShape.Circle), 32));
            _waves.Add(new Wave(wave3Unit, 16));
            _waves.Add(new Wave(new UnitPrototype(weapon25Damage      , 80f,   2f, 10f,  50f, Color.Brown  , UnitPrototype.DrawShape.Square), 8));
            _waves.Add(new Wave(new UnitPrototype(weapon10DamageRangedRapid, 400f, 0f, 15f, 100f, Color.DarkRed, UnitPrototype.DrawShape.Square), 1));

        }

        public void InitPlayer()
        {
            AssertUtil.NotNull(_controller);
            AssertUtil.NotNull(_controller.Input);
            AssertUtil.NotNull(Map);

            WeaponPrototypeBasic weapon = new WeaponPrototypeBasic(new ProjectileAction(new ProjectilePrototype(2.5f,
                600f,
                2f,
                new DamageAction(10f),
                Color.Orange)),
                0.20f,
                float.PositiveInfinity,3f);

            UnitPrototype playerUnit = new((WeaponPrototype)weapon, 100f, 1f, 100f, 3f, 5f, 150f, Color.Green, UnitPrototype.DrawShape.Circle);

            PlayerUnit = UnitFactory.SpawnUnit(playerUnit, (Vector2)Map.GetPlayerSpawn(), new PlayerController(_controller.Input), Team.Ally);


            AddUnit(PlayerUnit);
        }

        
        private bool CheckGameOverCondition(IBoardContext context)
        {
            AssertUtil.NotNull(PlayerUnit);
            if (PlayerUnit.IsExpired(context))
            {
                AssertUtil.NotNull(_controller);
                _controller.OnDefeat();
                return true;
            }
            return false;
        }
        private bool CheckVictoryCondition()
        {
            if (_waveCurrent == _waves.Count)
            {
                AssertUtil.NotNull(_controller);
                _controller.OnVictory();
                return true;
            }
            return false;
        }

        public void Update(float deltaTime)
        {
            if(deltaTime <= 0) return;
            CurrentTime += deltaTime;


            if (TimeUtils.HasElapsed(CurrentTime,_lastEnemySpawned,ENEMY_SPAWN_COOLDOWN))
            {
                if (_waveSpawnsRemaining > 0)
                {
                    _lastEnemySpawned = CurrentTime;
                    _waveSpawnsRemaining--;
                    Vector2 spawnPoint = spawnPoints[RandomUtils.RandomIntBelow(spawnPoints.Count)]; // Poor, but for now

                    Unit unit = UnitFactory.SpawnUnit(_waves[_waveCurrent].Unit,spawnPoint, new BasicAIController(), Team.Enemy);
                    unit.TargetUnit = PlayerUnit;
                    AddUnit(unit);
                    // Elite
                    if (_waveSpawnsRemaining > 0 && _waveSpawnsRemaining == _waves[_waveCurrent].CountToSpawn)
                    {
                        // unit.MaxHealth *= 4;
                        // unit.Health *= 4;
                        // unit.Speed += 25;
                        // unit.Damage *= 4;
                        // I probably need to... Do something to register units as unique.
                        // I can't just set these values.
                        
                    }
                }
                else if (!Units.Entities.Any(s => s.IsAlive && s.Team == Team.Enemy))
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

            // Separate required, because units can create projectiles.
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
            AssertUtil.NotNull(Map);
            foreach (Projectile projectile in Projectiles.Entities.Where(p => p.IsAlive))
            {
                foreach (Unit unit in Units.Entities.Where(p => p.IsAlive && p.Team.IsHostileTo(projectile.Team)))
                {
                    if (projectile.Intersects(unit))
                    {
                        projectile.Die(this);
                        projectile.ImpactEffect?.ExecuteOnEntity(projectile.Caster, projectile, unit, this);

                        break;
                    }
                }
            }

            foreach (Projectile projectile in Projectiles.Entities)
            {
                if (!projectile.BoundingBox.IntersectsWith(Map.WorldBounds))
                {
                    projectile.Die(this);
                }
            }
            AssertUtil.NotNull(PlayerUnit);
            if (!Map.WorldBounds.Contains(PlayerUnit.BoundingBox))
            {
                var halfSize = PlayerUnit.BoundingBox;

                PlayerUnit.Position = new Vector2(
                    Math.Clamp(PlayerUnit.Position.X, Map.WorldBounds.Left + halfSize.Width, Map.WorldBounds.Right - halfSize.Width),
                    Math.Clamp(PlayerUnit.Position.Y, Map.WorldBounds.Top + halfSize.Height, Map.WorldBounds.Bottom - halfSize.Height)
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

        public void Dispose()
        {
            DrawCache.Dispose();


            //Entities.Clear(); // Don't think I really need to do this anyway.
            //Units.Clear();
            //Projectiles.Clear();
        }

    }
}
