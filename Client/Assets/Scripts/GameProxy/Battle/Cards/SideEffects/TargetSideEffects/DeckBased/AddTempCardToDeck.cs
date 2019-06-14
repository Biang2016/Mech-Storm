namespace SideEffects
{
    public class AddTempCardToDeck : TargetSideEffect, ICardDeckLinked
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
            int cardID = M_SideEffectParam.GetParam_ConstInt("CardID");
            if (cardID == (int) AllCards.EmptyCardTypes.NoCard || cardID == (int) AllCards.EmptyCardTypes.EmptyCard)
            {
                return "Error!!!";
            }

            BaseInfo bi = AllCards.GetCard(cardID).BaseInfo;
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

        public SideEffectValue_ConstInt GetCardIDSideEffectValue()
        {
            return (SideEffectValue_ConstInt) M_SideEffectParam.GetParam("CardID");
        }
    }
}