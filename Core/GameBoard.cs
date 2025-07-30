using System.Data;
using System.Numerics;

using AssertUtils;

using AStar;
using AStar.Options;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Controllers;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Waves;

using static Wc3_Combat_Game.Core.GameConstants;
//using Wc3_Combat_Game.IO.Load.GameSchema;

namespace Wc3_Combat_Game.Core
{
    public class GameBoard: IBoardContext, IDrawContext, IDisposable
    {
        private GameController? _controller;
        public GameController Controller
        {
            get
            {
                AssertUtil.NotNull(_controller);
                return _controller;
            }
            private set => _controller = value;
        }

        public float CurrentTime { get; private set; } = 0f;

        //private List<PrototypeUnit> _waveUnits = new();
        //private List<int> _waveUnitCounts = new();

        //private List<Wave> _waves = new();
        private int CurrentWave => _waveManager.CurrentWave;
        //private int _waveSpawnsRemaining;

        private List<Vector2> spawnPoints = new();

        Camera IDrawContext.Camera => Controller.View.Camera;

        // Entities.
        public Unit? PlayerUnit { get; private set; }

        internal EntityManager<Projectile> Projectiles { get; private set; } = new();
        internal EntityManager<Unit> Units { get; private set; } = new();

        public EntityManager<Entity> Entities { get; private set; } = new();

        private float _lastCacheUpdateTime = float.NegativeInfinity;
        private Dictionary<Team, List<Unit>> _friendlyUnitsCache = new Dictionary<Team, List<Unit>>();
        private Dictionary<Team, List<Unit>> _enemyUnitsCache = new Dictionary<Team, List<Unit>>();


        public Map Map
        {
            get
            {
                AssertUtil.NotNull(_map);
                return _map;
            }
            private set => _map = value;
        }
        private Map? _map;
        public PathFinder PathFinder
        {
            get
            {
                AssertUtil.NotNull(_pathfinder);
                return _pathfinder;
            }
            private set => _pathfinder = value;
        }


        private PathFinder? _pathfinder;
        public float TileSize { get; private set; }

        public DebugSettings DebugSettings = new DebugSettings(); // May or may not stay here.
        DebugSettings IDrawContext.DebugSettings => DebugSettings;

        public DrawCache DrawCache { get; private set; }

        private PhysicsManager _physicsManager;

        public World PhysicsWorld { get => _physicsManager._world; }

        private WaveManager _waveManager;

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



        public GameBoard()
        {
            DrawCache = new DrawCache();
            _physicsManager = new PhysicsManager();
            _waveManager = new WaveManager();
            //_physicsManager.RegisterForm(Controller?.View.DebugPanel); // Register the form if available.
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

            Map.InitCollision(this);

        }

        public void InitWaves()
        {
            _waveManager = new WaveManager();
            _waveManager.InitWaves(this.spawnPoints);

            // Will be in wave class when we get there.
            //_waveCurrent = -1;
            //_waveSpawnsRemaining = 0;
            //
            //var meleeWeaponBase = new TargetedAbilityPrototype(null, 1f, 20f);
            //var weapon5Damage = meleeWeaponBase.WithDamage(5f);
            //var weapon10Damage = meleeWeaponBase.WithDamage(10f);
            //var weapon25Damage = meleeWeaponBase.WithDamage(25f);
            //var weapon200Damage = meleeWeaponBase.WithDamage(200f);
            //
            //var rangedWeaponBase = new TargetedAbilityPrototype(
            //    new ProjectileAction(new ProjectilePrototype("Ranged Weapon",2.5f, 225f, 4f, null, Color.DarkMagenta)),
            //    0.5f,
            //    150f,10f);
            //
            //var unit = new UnitPrototype(15f, 2f, 4f, 50f, Color.Brown, 6);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(5f));
            //_waves.Add(new Wave(unit, 32));
            //
            //unit = new UnitPrototype(10f, 0.0f, 4f, 75f, Color.DarkGoldenrod, 3);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(10f));
            //_waves.Add(new Wave(unit, 32));
            //
            //unit = new UnitPrototype(30f, 0.0f, 5f, 40f, Color.Orange, 5);
            //unit = unit.AddWeapon(rangedWeaponBase.WithDamage(10f));
            //_waves.Add(new Wave(unit, 16));
            //
            //unit = new UnitPrototype(80f, 2f, 10f, 50f, Color.Brown, 6);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(25f));
            //_waves.Add(new Wave(unit, 8));
            //
            //unit = new UnitPrototype(400f, 0f, 15f, 100f, Color.DarkRed, 4);
            //unit = unit.AddWeapon(meleeWeaponBase.WithDamage(90f));
            //unit = unit.AddWeapon(rangedWeaponBase.WithDamage(10f)); // Change to a ranged snare. Also, screw with other bits.
            //_waves.Add(new Wave(unit, 1));





            //var wave3Unit = new UnitPrototype(weapon10DamageRanged, 30f,   0f, 10f,0.5f, 5f,  40f, Color.Orange , UnitPrototype.DrawShape.Square);
            //
            ////_waves.Add(new Wave(new UnitPrototype(weapon5Damage       , 10f,   2f,  8f,  75f, Color.Brown  , UnitPrototype.DrawShape.Circle), 1));
            //_waves.Add(new Wave(new UnitPrototype(weapon5Damage, 12f, 2f, 4f, 50f, Color.Brown, UnitPrototype.DrawShape.Circle), 32));
            //_waves.Add(new Wave(new UnitPrototype(weapon10Damage, 10f, 0.1f, 4f, 75f, Color.Pink, UnitPrototype.DrawShape.Circle), 32));
            //_waves.Add(new Wave(wave3Unit, 16));
            //_waves.Add(new Wave(new UnitPrototype(weapon25Damage, 80f, 2f, 10f, 50f, Color.Brown, UnitPrototype.DrawShape.Square), 8));
            //_waves.Add(new Wave(new UnitPrototype(weapon10DamageRangedRapid, 400f, 0f, 15f, 100f, Color.DarkRed, UnitPrototype.DrawShape.Square), 1));

        }

        public void InitPlayer()
        {
            AssertUtil.NotNull(Controller);
            AssertUtil.NotNull(Controller.Input);

            TargetedAbilityPrototype weapon = new TargetedAbilityPrototype(new ProjectileAction(new ProjectilePrototype("Player Weapon",2.5f,
                600f,
                2f,
                new DamageAction(10f),
                Color.Orange)),
                0.20f,
                float.PositiveInfinity,3f);

            UnitPrototype playerUnit = new(100f,  3f, 5f, 150f, Color.Green, 0);
            playerUnit = playerUnit.AddWeapon(weapon);
            playerUnit = playerUnit.WithMana(100, 3f);
            PlayerUnit = UnitFactory.SpawnUnit(playerUnit, Map.GetPlayerSpawn(), new PlayerController(Controller.Input), Team.Ally, this);


            AddUnit(PlayerUnit);
        }


        private bool CheckGameOverCondition(IBoardContext context)
        {
            AssertUtil.NotNull(PlayerUnit);
            if(PlayerUnit.IsExpired(context))
            {
                AssertUtil.NotNull(Controller);
                Controller.OnDefeat();
                return true;
            }
            return false;
        }
        private bool CheckVictoryCondition()
        {
            if(_waveManager.AllWavesComplete)
            {
                AssertUtil.NotNull(Controller);
                Controller.OnVictory();
                return true;
            }
            return false;
        }

        public void Update(float deltaTime)
        {
            if(deltaTime <= 0) return;
            CurrentTime += deltaTime;

            //Projectiles.ForEach((projectile) =>
            //{
            //    Console.WriteLine($"306 - Entity[{projectile.Index}] Pos({projectile.Position}, Velocity({projectile.PhysicsObject.Body.LinearVelocity})");
            //});

            _waveManager.Update(deltaTime, this);
            CheckVictoryCondition();
            //if(TimeUtils.HasElapsed(CurrentTime, _lastEnemySpawned, ENEMY_SPAWN_COOLDOWN))
            //{
            //    if(_waveSpawnsRemaining > 0)
            //    {
            //        Vector2 spawnPoint = spawnPoints[RandomUtils.RandomIntBelow(spawnPoints.Count)]; // Poor, but for now
            //        Unit unit;
            //        if(_waveSpawnsRemaining == _waves[_waveCurrent].CountToSpawn && _waves[_waveCurrent].CountToSpawn > 1)
            //        {
            //            UnitPrototype elitePrototype = _waves[_waveCurrent].Unit;
            //            elitePrototype = elitePrototype.WithLife(elitePrototype.MaxLife * 4, elitePrototype.LifeRegen * 4);
            //            elitePrototype = elitePrototype.WithSpeed(elitePrototype.Speed * 1.2f);
            //            elitePrototype = elitePrototype.WithRadius(elitePrototype.Radius * 1.5f);
            //            //elitePrototype = elitePrototype.WithColors(Color.Black, elitePrototype.DamagedColor, //elitePrototype.DeadColor, elitePrototype.PolygonCount);
            //            AbilityPrototype[] abilities = new AbilityPrototype[elitePrototype.Abilities.Length];
            //            for (int x = 0; x < elitePrototype.Abilities.Length; x++)
            //            {
            //                AbilityPrototype ability = elitePrototype.Abilities[x];
            //                // hacky solution.
            //                if(ability is TargetedAbilityPrototype targetedAbility)
            //                {
            //                    float damage =  targetedAbility.GetDamage();
            //                    targetedAbility = targetedAbility.WithDamage(damage * 4);
            //                    abilities[x] = targetedAbility;
            //                }
            //            }
            //            elitePrototype = elitePrototype.WithAbilities(abilities);
            //            unit = UnitFactory.SpawnUnit(elitePrototype, spawnPoint, new BasicAIController(), Team.Enemy,this);
            //        }
            //        else
            //        {
            //            unit = UnitFactory.SpawnUnit(_waves[_waveCurrent].Unit, spawnPoint, new BasicAIController(), //Team.Enemy,this);
            //
            //        }
            //
            //        unit.TargetUnit = PlayerUnit;
            //        AddUnit(unit);
            //        _lastEnemySpawned = CurrentTime;
            //        _waveSpawnsRemaining--;
            //
            //    }
            //    else if(!Units.Entities.Any(s => s.IsAlive && s.Team == Team.Enemy))
            //    {
            //        // Should be .Count, but also needs a boss tag check. Later.
            //        // After all, we do next wave when less than 1/8th remain, or less than 8, maybe. dunno. 
            //        // Right now, its 100%.
            //        // New Wave
            //
            //        _waveCurrent++;
            //
            //        if(!CheckVictoryCondition())
            //        {
            //            _waveSpawnsRemaining = _waves[_waveCurrent].CountToSpawn;
            //        }
            //    }
            //
            //
            //}

            // Update Entities.

            Units.UpdateAll(deltaTime, this);
            Projectiles.UpdateAll(deltaTime, this);


            //Projectiles.ForEach((projectile) =>
            //{
            //    Console.WriteLine($"375 - Entity[{projectile.Index}] Pos({projectile.Position}, Velocity({projectile.PhysicsObject.Body.LinearVelocity})");
            //});
            _physicsManager.Update(deltaTime);

            //Projectiles.ForEach((projectile) =>
            //{
            //    Console.WriteLine($"380 - Entity[{projectile.Index}] Pos({projectile.Position}, Velocity({projectile.PhysicsObject.Body.LinearVelocity})");
            //});
            // Separate required, because units can create projectiles.
            // Admittedly, eventually projectiles will be able to create projectiles, Starburst or Delayed cast.
            // so a method to do this right is needed.

            //CheckCollision(deltaTime, this);

            //Projectiles.ForEach((projectile) =>
            //{
            //    Console.WriteLine($"385 - Entity[{projectile.Index}] Pos({projectile.Position}, Velocity({projectile.PhysicsObject.Body.LinearVelocity})");
            //});

            // Cleanup dead entities.
            Projectiles.RemoveExpired(this);
            Units.RemoveExpired(this);
            Entities.RemoveExpired(this);

            CheckGameOverCondition(this);

            //Projectiles.ForEach((projectile) =>
            //{
            //    Console.WriteLine($"395 - Entity[{projectile.Index}] Pos({projectile.Position}, Velocity({projectile.PhysicsObject.Body.LinearVelocity})");
            //});
            // End Logic
        }

        private void CheckCollision(float deltaTime, IBoardContext context)
        {
            foreach(Projectile projectile in Projectiles.Entities.Where(p => p.IsAlive))
            {
                foreach(Unit unit in Units.Entities.Where(p => p.IsAlive && p.Team.IsHostileTo(projectile.Team)))
                {
                    if(projectile.Intersects(unit))
                    {
                        projectile.Die(this);
                        projectile.ImpactEffect?.ExecuteOnEntity(projectile.Caster, projectile, unit, this);

                        break;
                    }
                }
            }

            foreach(Projectile projectile in Projectiles.Entities)
            {
                if(!projectile.BoundingBox.IntersectsWith(Map.WorldBounds))
                {
                    projectile.Die(this);
                }
            }
            AssertUtil.NotNull(PlayerUnit);
            //if(!Map.WorldBounds.Contains(PlayerUnit.BoundingBox))
            //{
            //    var halfSize = PlayerUnit.BoundingBox;
            //
            //    Vector2 newPosition = new Vector2(
            //        Math.Clamp(PlayerUnit.Position.X, Map.WorldBounds.Left + halfSize.Width, Map.WorldBounds.Right - halfSize.Width),
            //        Math.Clamp(PlayerUnit.Position.Y, Map.WorldBounds.Top + halfSize.Height, Map.WorldBounds.Bottom - halfSize.Height));
            //    PlayerUnit.Mover.Teleport(newPosition, context);
            //
            //}
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
            
            // Temporary fix, not sure where to do this.
            if(u.Team == Team.Enemy)
            {
                u.TargetUnit = PlayerUnit;
            }
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
