﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class HarassThought : ParallelThought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return ((IsKeyDown(context) && context.Target != null && context.Target.IsValid) || IsWithinWRange()) && !context.CastingUlt;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;

            if (IsKeyDown(context))
            {
                KatarinaUtilities.CastQ(context.Target);

                switch (context.Plugin.Config.Item("HarrassQWE").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (context.Plugin.Config.Item("HarassDetonateQ").GetValue<bool>() &&
                            Environment.TickCount >= context.QTimeToHit)
                        {
                            if (!context.Plugin.Q.IsReady())
                                KatarinaUtilities.CastE(context.Target);

                            if (!context.Plugin.E.IsReady())
                                KatarinaUtilities.CastW(context.Target);
                        }
                        else if (!context.Plugin.Config.Item("HarassDetonateQ").GetValue<bool>())
                        {
                            KatarinaUtilities.CastE(context.Target);
                            if (!context.Plugin.E.IsReady() && IsWithinWRange())
                                KatarinaUtilities.CastW(context.Target);
                        }
                        break;

                    case 1:
                        if (ObjectManager.Player.Distance(context.Target, true) <
                            Katarina.Instance.W.Range*Katarina.Instance.W.Range)
                            KatarinaUtilities.CastW(context.Target);
                        break;
                }
            }

            if (context.Plugin.Config.Item("WHarass").GetValue<bool>() && IsWithinWRange())
                KatarinaUtilities.CastW();
        }

        public bool IsKeyDown(ThoughtContext context)
        {
            return context.Plugin.Config.Item("HarassKey").GetValue<KeyBind>().Active;
        }

        public bool IsWithinWRange()
        {
            return ObjectManager.Get<Obj_AI_Hero>().Any(x => x.IsEnemy && x.IsValidTarget(Katarina.Instance.W.Range));
        }
    }
}