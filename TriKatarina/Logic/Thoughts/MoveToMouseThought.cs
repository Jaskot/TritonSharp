﻿using LeagueSharp;
using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    public class MoveToMouseThought : ParallelThought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext) contextObj;
            return !context.CastingUlt && ShouldMove();
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        public bool ShouldMove()
        {
            return (IsHarassKeyDown() && ShouldHarassMoveToMouse()) || (IsComboKeyDown() && ShouldComboMoveToMouse()) || IsWardJumpKeyDown();
        }

        public bool IsHarassKeyDown()
        {
            return Katarina.Instance.Config.Item("HarassKey").GetValue<KeyBind>().Active;            
        }

        public bool ShouldHarassMoveToMouse()
        {
            return Katarina.Instance.Config.Item("HarassMoveToMouse").GetValue<bool>();            
        }

        public bool IsComboKeyDown()
        {
            return Katarina.Instance.Config.Item("ComboKey").GetValue<KeyBind>().Active;
        }

        public bool ShouldComboMoveToMouse()
        {
            return Katarina.Instance.Config.Item("ComboMoveToMouse").GetValue<bool>();
        }

        public bool IsWardJumpKeyDown()
        {
            return Katarina.Instance.Config.Item("WardJumpKey").GetValue<KeyBind>().Active;
        }
    }
}
