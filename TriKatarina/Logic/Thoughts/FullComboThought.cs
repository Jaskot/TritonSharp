using System;
using LeagueSharp.Common;
using Triton;
using Triton.Constants;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class FullComboThought : Thought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return context.Plugin.Config.Item("ComboKey").GetValue<KeyBind>().Active && context.Target != null && context.Target.IsValid && (!context.CastingUlt || context.Plugin.Config.Item("StopUlt").GetValue<bool>());
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;

            UseItems(context);

            KatarinaUtilities.CastQ(context.Target);

            if (context.Plugin.Config.Item("ComboDetonateQ").GetValue<bool>() && Environment.TickCount >= context.QTimeToHit)
            {
                if (!context.Plugin.Q.IsReady())
                    KatarinaUtilities.CastE(context.Target);

                if (!context.Plugin.E.IsReady())
                    KatarinaUtilities.CastW(context.Target);
            }
            else
            {
                KatarinaUtilities.CastE(context.Target);
                if (!context.Plugin.E.IsReady())
                    KatarinaUtilities.CastW(context.Target);
            }

            KatarinaUtilities.CastR();
        }

        private void UseItems(ThoughtContext context)
        {
            if (Items.CanUseItem((int)ItemIds.DeathfireGrasp))
                Items.UseItem((int) ItemIds.DeathfireGrasp, context.Target);
        }
    }
}
