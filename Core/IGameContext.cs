using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Core
{
    public interface IBoardContext
    {
        float CurrentTime { get; }
        void AddProjectile(Projectile p);
        void AddUnit(Unit u);
    }

    public class BoardContext : IBoardContext
    {
        readonly GameBoard _board;

        public BoardContext(GameBoard board)
        {
            _board = board;
        }

        public float CurrentTime => _board.CurrentTime;

        public void AddProjectile(Projectile p) => _board.Projectiles.Add(p);
        public void AddUnit(Unit u) => _board.Units.Add(u);

    }
}
