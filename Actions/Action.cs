using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Effects
{
    public class Action
    {

        //public enum TARGET
        //{
        //    SELF,
        //    RUNNER,
        //    TOWER
        //}

        protected virtual void Execute(Entities.Entity? Source, IBoardContext context)
        { }

        public virtual void ApplyToEntity(Entities.Entity? Caster, Entities.Entity? Emitter, Entities.Entity Target, IBoardContext context)
        { }

        public virtual void ApplyToPoint(Entities.Entity? Caster, Entities.Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        { }

        //protected static bool validateTarget(TARGET iType, Entity iTarget)
        //{
        //    if (iType == TARGET.RUNNER && iTarget is Runner
        //        || iType == TARGET.TOWER && iTarget is Tower)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
