using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TriKatarina.Logic;
using TriKatarina.Logic.Thoughts;
using Triton;
using Triton.Constants;

namespace TriKatarina
{
    public static class KatarinaUtilities
    {
        private static readonly int _wardDistance = 300;

        public static float QTimeToHit { get; set; }
        public static float LastWardJump { get; set; }

        public static bool CastQ(Obj_AI_Base target)
        {
            if (target == null || !target.IsValid || !Katarina.Instance.Q.IsReady() || !target.IsValidTarget(Katarina.Instance.Q.Range))
                return false;

            Katarina.Instance.Q.CastOnUnit(target, true);
            if (QTimeToHit == 0 || Environment.TickCount >= QTimeToHit)
            {
                QTimeToHit = Environment.TickCount +
                                     (Katarina.Instance.Q.Delay +
                                      (ObjectManager.Player.Distance(target.ServerPosition, false) /
                                       Katarina.Instance.Q.Speed));
            }
            return true;
        }

        public static bool CastE(Obj_AI_Base target)
        {
            if (target == null || !target.IsValid || !Katarina.Instance.E.IsReady() || !target.IsValidTarget(Katarina.Instance.E.Range))
                return false;

            Katarina.Instance.E.CastOnUnit(target, true);

            return true;
        }

        public static bool CastW(Obj_AI_Base target)
        {
            if (target == null || !target.IsValid || !Katarina.Instance.W.IsReady() || !target.IsValidTarget(Katarina.Instance.W.Range))
                return false;

            return Katarina.Instance.W.Cast();

        }

        public static bool CastW()
        {
            if (!Katarina.Instance.W.IsReady())
                return false;

            return Katarina.Instance.W.Cast();
        }

        public static bool CastR()
        {
            if (Katarina.Instance.Q.IsReady() || Katarina.Instance.W.IsReady() || Katarina.Instance.E.IsReady() || !Katarina.Instance.R.IsReady() ||
                ObjectManager.Get<Obj_AI_Hero>().Count(x => x.IsValidTarget(Katarina.Instance.R.Range)) < 1)
                return false;

            return Katarina.Instance.R.Cast();
        }

        public static bool WardJump(ThoughtContext context, float x, float y)
        {
            if (!context.Plugin.E.IsReady())
                return false;

            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(z => z.IsAlly))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(z => z.IsAlly))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(z => !z.IsAlly))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Base>().Where(IsWard))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            if (Environment.TickCount >= LastWardJump)
            {
                var wardSlot = Items.GetWardSlot();

                if (wardSlot != null)
                {
                    wardSlot.UseItem(new Vector3(x, y, 0));
                    LastWardJump = Environment.TickCount + 2000;
                }
            }

            return true;
        }

        public static void CalculateDamage(ThoughtContext context)
        {
            if (context.Target == null || !context.Target.IsValid)
                return;

            context.DamageContext.PDamage = Katarina.Instance.Q.IsReady() ? DamageLib.getDmg(context.Target, DamageLib.SpellType.Q, DamageLib.StageType.FirstDamage) : 0;
            context.DamageContext.QDamage = Katarina.Instance.Q.IsReady() ? DamageLib.getDmg(context.Target, DamageLib.SpellType.Q) : 0;
            context.DamageContext.WDamage = Katarina.Instance.W.IsReady() ? DamageLib.getDmg(context.Target, DamageLib.SpellType.W) : 0;
            context.DamageContext.EDamage = Katarina.Instance.E.IsReady() ? DamageLib.getDmg(context.Target, DamageLib.SpellType.E) : 0;
            context.DamageContext.RDamage = Katarina.Instance.R.IsReady() ? DamageLib.getDmg(context.Target, DamageLib.SpellType.R, DamageLib.StageType.FirstDamage) : 0;

            context.DamageContext.DFGDamage = Items.CanUseItem((int)ItemIds.DeathfireGrasp) ? DamageLib.getDmg(context.Target, DamageLib.SpellType.DFG) : 0;
            context.DamageContext.HXGDamage = Items.CanUseItem((int)ItemIds.HextechGunblade) ? DamageLib.getDmg(context.Target, DamageLib.SpellType.HEXGUN) : 0;
            context.DamageContext.BWCDamage = Items.CanUseItem((int)ItemIds.BilgewaterCutlass) ? DamageLib.getDmg(context.Target, DamageLib.SpellType.BILGEWATER) : 0;
            context.DamageContext.LiandrysDamage = Items.HasItem((int)ItemIds.LiandrysTorment) ? GetLiandrysDamage(context) : 0;

        }

        public static double GetLiandrysDamage(ThoughtContext context)
        {
            return DamageLib.CalcMagicDmg(0.06 * context.Target.MaxHealth, context.Target);
        }

        public static bool IsWard(Obj_AI_Base obj)
        {
            return obj.Name.Contains("Ward") || obj.Name.Contains("Wriggle") || obj.Name.Contains("Trinket");
        }

        public static bool IsValidJumpTarget(Obj_AI_Base obj)
        {
            return obj != null && ((obj.IsValidTarget(Katarina.Instance.E.Range, false) && obj.Distance(Game.CursorPos, true) <= _wardDistance*_wardDistance) || (obj.IsValid && IsWard(obj) && obj.Distance(GetMousePosition(), true) <= 300*300));
        }

        public static Vector3 GetMousePosition()
        {
            var range = 600;
            var myPos = ObjectManager.Player.ServerPosition;
            var mousePos = Game.CursorPos;

            var norm = (myPos - mousePos);
            norm.Normalize();

            return myPos - norm * range;
        }
    }
}
