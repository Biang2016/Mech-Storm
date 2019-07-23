namespace SideEffects
{
    public class AddEnergy : TargetSideEffect, IShipEnergy
    {
        public AddEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
            M_SideEffectParam.SetParam_ConstInt("Times", 1);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.ShipBased;

        public override string GenerateDesc()
        {
            int times = M_SideEffectParam.GetParam_ConstInt("Times");

            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Energy"),
                times > 1 ? ("*" + M_SideEffectParam.GetParam_ConstInt("Times")) : "");
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            int times = M_SideEffectParam.GetParam_ConstInt("Times");
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp)
                {
                    for (int i = 0; i < times; i++)
                    {
                        sp.AddEnergy(value);
                    }
                },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }
    }
}