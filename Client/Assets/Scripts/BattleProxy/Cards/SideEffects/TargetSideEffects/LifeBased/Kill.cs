namespace SideEffects
{
    public class Kill : TargetSideEffect, INegative, IPriorUsed
    {
        public Kill()
        {
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange());
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.KillMechs(ChoiceCount, executorInfo.TargetMechIds, player, TargetRange, TargetSelect, -1);
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return -5;
        }
    }
}