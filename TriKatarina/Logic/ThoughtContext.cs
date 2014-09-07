using LeagueSharp;
using LeagueSharp.Common;
using Triton.Plugins;

namespace TriKatarina.Logic
{
    public class ThoughtContext
    {
        private DamageContext _damageContext;
        private bool _castingUlt;

        public Obj_AI_Hero Target { get; set; }
        public ChampionPluginBase Plugin { get; set; }

        public float QTimeToHit { get; set; }

        public bool CastingUlt
        {
            get
            {
                return _castingUlt || ObjectManager.Player.IsChannelingImportantSpell();
            }
            set { _castingUlt = value; }
        }

        public DamageContext DamageContext
        {
            get { return _damageContext ?? (_damageContext = new DamageContext()); }
        }
    }
}