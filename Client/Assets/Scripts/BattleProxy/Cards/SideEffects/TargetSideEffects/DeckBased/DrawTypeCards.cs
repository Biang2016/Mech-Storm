namespace SideEffects
{
    public class DrawTypeCards : TargetSideEffect, IPriorUsed, IPositive
    {
        public DrawTypeCards()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
            M_SideEffectParam.SetParam_ConstInt("DrawCardType", (int) CardTypes.Energy, typeof(CardTypes));
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(
                DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"),
                BaseInfo.CardTypeNameDict[LanguageManager_Common.GetCurrentLanguage()][(CardTypes) M_SideEffectParam.GetParam_ConstInt("DrawCardType")],
                M_SideEffectParam.GetParam_MultipliedInt("CardCount") <= 1 ? "" : "s");
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp) { sp.HandManager.DrawCardsByType((CardTypes) M_SideEffectParam.GetParam_ConstInt("DrawCardType"), M_SideEffectParam.GetParam_MultipliedInt("CardCount")); },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return 3 * M_SideEffectParam.GetParam_MultipliedInt("CardCount");
        }
    }
}