using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Core
{
    public class PhysicsManager
    {
        public const Category PlayerCategory = Category.Cat1;
        public const Category PlayerProjectileCategory = Category.Cat2;
        public const Category EnemyCategory = Category.Cat3;
        public const Category EnemyProjectileCategory = Category.Cat4;
        public const Category TerrainCategory = Category.Cat5;

        public World _world;

        public PhysicsManager()
        {
            _world = new World(Vector2.Zero);
        }

        public void Update(float deltaTime)
        {
            _world.Step(deltaTime);
        }
    }
}