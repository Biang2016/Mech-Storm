namespace SideEffects
{
    public class DrawCards : TargetSideEffect
    {
        public DrawCards()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(
                DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount") <= 1 ? "" : "s");
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.HandManager.DrawCards(M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }
    }
}