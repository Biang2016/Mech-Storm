namespace SideEffects
{
    public class WaitInHandDecreaseEnergy : HandCardRelatedSideEffect, IPostUsed
    {
        public WaitInHandDecreaseEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("DecValue", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("DecValue"));
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer sp = (BattlePlayer) Player;
            CardBase ci = sp.HandManager.GetCardByCardInstanceId(executorInfo.CardInstanceId);

            if (ci.CardInfo.BaseInfo.Energy >= M_SideEffectParam.GetParam_MultipliedInt("DecValue"))
            {
                ci.M_Energy -= M_SideEffectParam.GetParam_MultipliedInt("DecValue");
            }
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return 0;
        }
    }
}