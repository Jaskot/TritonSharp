using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class FarmThought : Thought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return context.Plugin.Config.Item("FarmKey").GetValue<KeyBind>().Active;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            var qFarm = context.Plugin.Config.Item("QFarm").GetValue<bool>();
            var wFarm = context.Plugin.Config.Item("WFarm").GetValue<bool>();
            var eFarm = context.Plugin.Config.Item("EFarm").GetValue<bool>();
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x != null && x.IsValid && x.IsEnemy))
            {

                var pDmg = DamageLib.getDmg(minion, DamageLib.SpellType.Q, DamageLib.StageType.FirstDamage);
                var qDmg = DamageLib.getDmg(minion, DamageLib.SpellType.Q);
                var wDmg = DamageLib.getDmg(minion, DamageLib.SpellType.W);
                var eDmg = DamageLib.getDmg(minion, DamageLib.SpellType.E);

                if (minion.IsValidTarget(context.Plugin.W.Range))
                {

                    if (qFarm && wFarm)
                    {
                        if (minion.Health <= (qDmg + wDmg) && minion.Health > wDmg && context.Plugin.Q.IsReady() &&
                            context.Plugin.W.IsReady())
                        {
                            KatarinaUtilities.CastQ(minion);
                            KatarinaUtilities.CastW();
                            break;
                        }
                        else if (context.Plugin.W.IsReady() && minion.Health <= wDmg)
                        {
                            KatarinaUtilities.CastW();
                            break;
                        }
                        else if (context.Plugin.Q.IsReady() && minion.Health <= qDmg)
                        {
                            KatarinaUtilities.CastQ(minion);
                            break;
                        }
                    }
                    else if (qFarm && context.Plugin.Q.IsReady() && minion.Health <= qDmg)
                    {
                        KatarinaUtilities.CastQ(minion);
                        break;
                    }
                    else if (wFarm && context.Plugin.W.IsReady() && minion.Health <= wDmg)
                    {
                        KatarinaUtilities.CastW();
                        break;
                    }
                }
                else
                {
                    if (qFarm && minion.Health <= qDmg && minion.IsValidTarget(context.Plugin.Q.Range))
                    {
                        KatarinaUtilities.CastQ(minion);
                        break;
                    }
                    if (eFarm && minion.Health <= eDmg && minion.IsValidTarget(context.Plugin.E.Range))
                    {
                        KatarinaUtilities.CastE(minion);
                        break;
                    }
                }
            }
        }
    }
}
