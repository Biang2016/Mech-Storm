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
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("LifeValue"));
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
            if (value < 0) return true;
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
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
        }
    }
}