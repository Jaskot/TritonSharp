using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Constants;

namespace TriKatarina.Logic
{
    public class Target
    {
        private Obj_AI_Hero _target;
        private DamageContext _damageContext = new DamageContext();
        private List<float> _distances = new List<float>();

        public Target(Obj_AI_Hero target)
        {
            _target = target;
        }

        public void CalculateDamage()
        {
            if (_target == null || !_target.IsValid)
                return;

            _damageContext.PDamage = Katarina.Instance.Q.IsReady() ? DamageLib.getDmg(_target, DamageLib.SpellType.Q, DamageLib.StageType.FirstDamage) : 0;
            _damageContext.QDamage = Katarina.Instance.Q.IsReady() ? DamageLib.getDmg(_target, DamageLib.SpellType.Q) : 0;
            _damageContext.WDamage = Katarina.Instance.W.IsReady() ? DamageLib.getDmg(_target, DamageLib.SpellType.W) : 0;
            _damageContext.EDamage = Katarina.Instance.E.IsReady() ? DamageLib.getDmg(_target, DamageLib.SpellType.E) : 0;
            _damageContext.RDamage = Katarina.Instance.R.IsReady() ? DamageLib.getDmg(_target, DamageLib.SpellType.R, DamageLib.StageType.FirstDamage) : 0;

            _damageContext.DFGDamage = Items.CanUseItem((int)ItemIds.DeathfireGrasp) ? DamageLib.getDmg(_target, DamageLib.SpellType.DFG) : 0;
            _damageContext.HXGDamage = Items.CanUseItem((int)ItemIds.HextechGunblade) ? DamageLib.getDmg(_target, DamageLib.SpellType.HEXGUN) : 0;
            _damageContext.BWCDamage = Items.CanUseItem((int)ItemIds.BilgewaterCutlass) ? DamageLib.getDmg(_target, DamageLib.SpellType.BILGEWATER) : 0;
            _damageContext.LiandrysDamage = Items.HasItem((int)ItemIds.LiandrysTorment) ? KatarinaUtilities.GetLiandrysDamage(this) : 0;

            var ignite = ObjectManager.Player.SummonerSpellbook.Spells.FirstOrDefault(x => x.Name == "summonerdot");
            _damageContext.IgniteDamage = ignite != null && ignite.State == SpellState.Ready ? DamageLib.getDmg(_target, DamageLib.SpellType.IGNITE) : 0;

        }

        public bool CanKill
        {
            get { return !_target.IsDead && _target.Health <= _damageContext.TotalDamage; }
        }

        public bool IgniteCanKill
        {
            get { return _target.Health < _damageContext.IgniteDamage && _target.IsValidTarget(600); }
        }

        public bool IsRunningAway
        {
            get
            {
                return false;
            }
        }

        public Obj_AI_Hero Unit
        {
            get { return _target; }
        }

        public DamageContext DamageContext
        {
            get { return _damageContext; }
        }
    }
}
