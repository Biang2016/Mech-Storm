namespace SideEffects
{
    public class WaitInHandDecreaseEnergy : HandCardRelatedSideEffect
    {
        public WaitInHandDecreaseEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("DecValue", 0);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("DecValue"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer sp = (BattlePlayer) Player;
            CardBase ci = sp.HandManager.GetCardByCardInstanceId(TargetCardInstanceId);

            if (ci.CardInfo.BaseInfo.Energy >= M_SideEffectParam.GetParam_MultipliedInt("DecValue"))
            {
                ci.M_Energy -= M_SideEffectParam.GetParam_MultipliedInt("DecValue");
            }
        }
    }
}