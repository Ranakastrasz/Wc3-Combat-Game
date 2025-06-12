using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Components.Actions.Interface
{

    public interface IGameplayAction
    {

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context);
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context);
    }

    public class GameplayActionNull : IGameplayAction
    {
        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            // Do nothing.
        }
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        {
            // Do nothing.
        }
    }
}
