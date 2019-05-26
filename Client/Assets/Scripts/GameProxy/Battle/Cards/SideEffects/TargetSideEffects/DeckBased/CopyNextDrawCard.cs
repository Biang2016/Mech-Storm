namespace SideEffects
{
    public class CopyNextDrawCard : TargetSideEffect
    {
        public CopyNextDrawCard()
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
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            CardInfo_Base ci = player.CardDeckManager.CardDeck.GetFirstCardInfo();
            if (ci != null)
            {
                player.HandManager.DrawCards(1);
                for (int i = 0; i < M_SideEffectParam.GetParam_MultipliedInt("CardCount"); i++)
                {
                    player.HandManager.GetACardByID(ci.CardID);
                }
            }
        }
    }
}