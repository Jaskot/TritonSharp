using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class AcquireTargetThought : ParallelThought
    {
        public override bool ShouldActualize(object contextObj)
        {
            return true;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            context.Target = SimpleTs.GetTarget(1200f, SimpleTs.DamageType.Magical);
        }
    }
}
