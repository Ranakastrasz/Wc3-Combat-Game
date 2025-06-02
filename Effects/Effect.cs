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
    public class Effect
    {

        //public enum TARGET
        //{
        //    SELF,
        //    RUNNER,
        //    TOWER
        //}

        protected virtual void Execute(IEntity? Source, IBoardContext context)
        { }

        public virtual void ApplyToEntity(IEntity? Caster, IEntity? Emitter, IEntity Target, IBoardContext context)
        { }

        public virtual void ApplyToPoint(IEntity? Caster, IEntity? Emitter, Vector2 TargetPoint, IBoardContext context)
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
