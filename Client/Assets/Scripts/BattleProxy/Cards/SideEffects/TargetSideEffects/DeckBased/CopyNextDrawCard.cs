namespace SideEffects
{
    public class CopyNextDrawCard : TargetSideEffect, IPriorUsed, IPositive
    {
        public CopyNextDrawCard()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(
                DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp)
                {
                    CardInfo_Base ci = sp.CardDeckManager.CardDeck.GetFirstCardInfo();
                    if (ci != null)
                    {
                        sp.HandManager.DrawCards(1);
                        for (int i = 0; i < M_SideEffectParam.GetParam_MultipliedInt("CardCount"); i++)
                        {
                            sp.HandManager.GetACardByID(ci.CardID);
                        }
                    }
                },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("CardCount") * 3;
        }
    }
}