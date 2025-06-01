using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Effects
{
    internal abstract class Effect
    {
        // Effects are an awful lot like Prototypes.
        // They are just a collection of data on how to do something, rather than a concrete object.

        protected bool _destroySource = false;

        public enum TARGET
        {
            SELF,
            RUNNER,
            TOWER
        }

        protected abstract void Execute(IEntity Source, float currentTime);

        public abstract void ApplyToEntity(IEntity Caster, IEntity Emitter, IEntity Target, float currentTime);

        public abstract void ApplyToPoint(IEntity Caster, IEntity Emitter, Vector2 TargetPoint, float currentTime);

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
