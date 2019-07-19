using System.Collections.Generic;

namespace SideEffects
{
    public class BreakEquip : TargetSideEffectEquip, IWeaken, IPriorUsed
    {
        public BreakEquip()
        {
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange());
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.EquipBased;
        public override List<TargetSelect> ValidTargetSelects => new List<TargetSelect> {TargetSelect.Single, TargetSelect.SingleRandom};

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.RemoveEquipByEquipID(executorInfo.TargetEquipIds[0]);
        }
    }
}