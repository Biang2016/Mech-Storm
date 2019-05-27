namespace SideEffects
{
    public class AddTempCardToDeck : TargetSideEffect
    {
        public AddTempCardToDeck()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
            M_SideEffectParam.SetParam_ConstInt("CardID", 0, typeof(CardDeck));
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("CardID")).BaseInfo;
            return HighlightStringFormat(
                DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"), 
                "[" + bi.CardNames[LanguageManager_Common.GetCurrentLanguage()] + "]");
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.CardDeckManager.RandomInsertTempCard(M_SideEffectParam.GetParam_ConstInt("CardID"), M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }
    }
}