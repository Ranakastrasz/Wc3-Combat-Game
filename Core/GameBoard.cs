using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Effects;
using static Wc3_Combat_Game.Core.GameConstants;
using Wc3_Combat_Game.Interface.Weapons;
using Wc3_Combat_Game.Interface.Controllers;
using Wc3_Combat_Game.Prototype;

namespace Wc3_Combat_Game.Core
{
    public class GameBoard : IBoardContext
    {
        private readonly GameController _controller;
        public BoardContext BoardContext;

        public float CurrentTime { get; private set; } = 0f;

        private List<PrototypeUnit> _waveUnits = new();
        private List<int> _waveUnitCounts = new();
        private int _waveCurrent;
        private int _waveSpawnsRemaining;


        // Entities.
        internal Unit MainPlayer { get; private set; }

        internal EntityManager<Projectile> Projectiles { get; private set; } = new();
        internal EntityManager<Unit> Units { get; private set; } = new();

        float IBoardContext.CurrentTime => throw new NotImplementedException();

        private float _lastEnemySpawned = 0f;


        public GameBoard(GameController controller)
        {
            _controller = controller;
            BoardContext = new BoardContext(this);


            // Init player
            PrototypeWeaponBasic weapon = new PrototypeWeaponBasic(new EffectProjectile(new PrototypeProjectile(5f,
            600f,
            2f,
            new EffectDamage(10f),
            Color.Blue)),
            0.20f,
            float.PositiveInfinity);


            PrototypeUnit playerUnit = new((PrototypeWeapon)weapon, 100f, 0.1f, 10f, 150f, Color.Green, PrototypeUnit.DrawShape.Circle);

            MainPlayer = UnitFactory.SpawnUnit(playerUnit, (Vector2)GAME_BOUNDS.Center(), new IPlayerController(controller.Input), TeamType.Ally);

            

            BoardContext.AddUnit(MainPlayer);


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

            //
            //public static UnitPrototype ENEMY_SWARMER_LIGHT   = new(   5f, 2f  ,  20f, 125f, Brushes.Red, UnitPrototype.DrawShape.Circle);
            //public static UnitPrototype ENEMY_SWARMER         = new(  10f, 0.1f,  30f, 150f, Brushes.Red, UnitPrototype.DrawShape.Circle);
            //public static UnitPrototype ENEMY_CASTER          = new(  30f, 0.1f,  30f, 100f, Brushes.Orange, UnitPrototype.DrawShape.Square);
            //public static UnitPrototype ENEMY_BRUTE           = new(  80f, 2f  ,  50f, 125f, Brushes.Red, UnitPrototype.DrawShape.Square);
            //public static UnitPrototype ENEMY_BRUTE_BOSS      = new(1000f, 0f  , 100f, 125f, Brushes.DarkRed, UnitPrototype.DrawShape.Square);

            //Unit              Type    HP      Dmg     Qty      Notes
            //Zombie	        Swarmer  5	    5-10    64       1 regen, trivial
            //Ghoul	            Swarmer 10	    10-20   64       Common
            //Acolyte	        Caster  30	    10-15   32       10 dmg spellbolt
            //Abomination	    Brute   80	    25-30   16       2 regen, ~25 dmg, elite 1-shots
            //Meat Golem	    Boss    2000    200     1        Berserk + haste, health degenerates when low

            //Skeleton Warrior	Swarmer 20	    10-15   128      Swarm type
            //Skeleton Archer   Caster  25	    10-15   64       25 dmg spellbolt, elite 1-shots
            //Skeleton Ork      Brute   100	    30-40   16       Tough brawler
            //Skeleton Mage	    Caster  320	    25-30   8        Rare, 100 dmg bolt + snare, hitscan
            //Lich Boss	        Boss    4000	200     1        Fan of 5 × 100 dmg bolts, snare, summon units
        }




        private void CheckGameOverCondition(BoardContext context)
        {
            if (MainPlayer.IsExpired(context))
            {
                _controller.OnGameOver();
            }
        }

        public void Update(float deltaTime)
        {
            CurrentTime += deltaTime;


            if (TimeUtils.HasElapsed(BoardContext.CurrentTime,_lastEnemySpawned,ENEMY_SPAWN_COOLDOWN))
            {
                if (_waveSpawnsRemaining > 0)
                {
                    _lastEnemySpawned = BoardContext.CurrentTime;
                    _waveSpawnsRemaining--;
                    Unit unit = UnitFactory.SpawnUnit(_waveUnits[_waveCurrent], (Vector2)RandomUtils.RandomPointBorder(SPAWN_BOUNDS), new IBasicAIController(), TeamType.Enemy);
                    unit.Target = MainPlayer;
                    BoardContext.AddUnit(unit);
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

            MainPlayer.Update(deltaTime, BoardContext);
            Projectiles.UpdateAll(deltaTime, BoardContext);
            Units.UpdateAll(deltaTime, BoardContext);

            CheckCollision(deltaTime);


            // Cleanup dead entities.
            Projectiles.RemoveExpired(BoardContext);
            Units.RemoveExpired(BoardContext);


            CheckGameOverCondition(BoardContext);
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
                        projectile.Die(BoardContext);
                        projectile.ImpactEffect?.ApplyToEntity(projectile.Caster, projectile, unit, BoardContext);

                        break;
                    }
                }
            }

            foreach (Projectile projectile in Projectiles.Entities)
            {
                if (!projectile.BoundingBox.IntersectsWith(GAME_BOUNDS))
                {
                    projectile.Die(BoardContext);
                }
            }

            if (!GAME_BOUNDS.Contains(MainPlayer.BoundingBox))
            {
                var bounds = GAME_BOUNDS;
                var halfSize = new SizeF(MainPlayer.Size / 2f, MainPlayer.Size / 2f);

                MainPlayer.Position = new Vector2(
                    Math.Clamp(MainPlayer.Position.X, bounds.Left + halfSize.Width, bounds.Right - halfSize.Width),
                    Math.Clamp(MainPlayer.Position.Y, bounds.Top + halfSize.Height, bounds.Bottom - halfSize.Height)
                );
            }
        }

        void IBoardContext.AddProjectile(Projectile p)
        {
            Projectiles.Add(p);
        }

        void IBoardContext.AddUnit(Unit u)
        {
            Units.Add(u);
        }
    }
}
