namespace SideEffects
{
    public class AddEnergyForEachMech : TargetSideEffect, IShipEnergy
    {
        public AddEnergyForEachMech()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.EveryMechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Energy"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            player.GameManager.SideEffect_ForeachMech(
                delegate(int validTargetCount) { player.AddEnergy(value * validTargetCount); },
                player,
                TargetRange);
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("Energy") * 3;
        }
    }
}