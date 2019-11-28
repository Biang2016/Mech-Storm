namespace SideEffects
{
    public class AddShield : TargetSideEffect, IStrengthen
    {
        public AddShield()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("ShieldValue", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("ShieldValue"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ShieldValue");
            if (value < 0) return;
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.GameManager.GetMech(executorInfo.MechId).M_MechShield += value;
            }

            player.GameManager.SideEffect_MechAction(
                delegate(ModuleMech mech) { mech.M_MechShield += value; },
                player,
                0,
                executorInfo.TargetMechIds,
                TargetRange,
                TargetSelect,
                -1
            );
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("ShieldValue") * 3;
        }
    }
}