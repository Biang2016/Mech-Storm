namespace SideEffects
{
    public class SummonMech : TargetSideEffect, ICardDeckLinked, IPositive
    {
        public SummonMech()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_ConstInt("SummonCardID", 0, typeof(CardDeck));
            M_SideEffectParam.SetParam_ConstInt("NumberOfSummon", 1);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.ShipBased;

        public override string GenerateDesc()
        {
            int cardID = M_SideEffectParam.GetParam_ConstInt("SummonCardID");
            int count = M_SideEffectParam.GetParam_ConstInt("NumberOfSummon");
            if (!AllCards.CardDict.ContainsKey(cardID))
            {
                return "Error!!!";
            }

            BaseInfo bi = AllCards.GetCard(cardID).BaseInfo;
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], bi.CardNames[LanguageManager_Common.GetCurrentLanguage()], count <= 1 ? "" : ("*" + count), GetDescOfTargetRange());
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int count = M_SideEffectParam.GetParam_ConstInt("NumberOfSummon");

            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp)
                {
                    for (int i = 0; i < count; i++)
                    {
                        sp.BattleGroundManager.AddMech((CardInfo_Mech) AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")));
                    }
                },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }

        public SideEffectValue_ConstInt GetCardIDSideEffectValue()
        {
            return (SideEffectValue_ConstInt) M_SideEffectParam.GetParam("SummonCardID");
        }
    }
}