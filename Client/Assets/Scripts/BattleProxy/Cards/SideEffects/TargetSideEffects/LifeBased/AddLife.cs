namespace SideEffects
{
    public class AddLife : TargetSideEffect, IStrengthen
    {
        public AddLife()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("LifeValue", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.LifeBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("LifeValue"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
            if (value < 0) return;
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.BattleGroundManager.GetMech(executorInfo.MechId).AddLife(value);
            }
            else
            {
                player.GameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.AddLife(value); },
                    player,
                    ChoiceCount,
                    TargetRange,
                    TargetSelect,
                    executorInfo.TargetClientIds,
                    executorInfo.TargetMechIds
                );
            }
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
        }
    }
}