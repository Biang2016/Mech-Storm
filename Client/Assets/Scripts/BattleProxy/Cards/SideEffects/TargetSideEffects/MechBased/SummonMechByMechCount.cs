using System.Collections.Generic;

namespace SideEffects
{
    public class SummonMechByMechCount : TargetSideEffect, ICardDeckLinked
    {
        public SummonMechByMechCount()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_ConstInt("SummonCardID", 0, typeof(CardDeck));
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.EveryMechBased;
        public override List<TargetSelect> ValidTargetSelects => new List<TargetSelect> {TargetSelect.Single};

        public override string GenerateDesc()
        {
            int cardID = M_SideEffectParam.GetParam_ConstInt("SummonCardID");
            if (cardID == (int) AllCards.EmptyCardTypes.NoCard || cardID == (int) AllCards.EmptyCardTypes.EmptyCard)
            {
                return "Error!!!";
            }

            BaseInfo bi = AllCards.GetCard(cardID).BaseInfo;
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = player.GameManager.CountMechsByTargetRange(TargetRange, player);

            for (int i = 0; i < value; i++)
            {
                int summonCardID = M_SideEffectParam.GetParam_ConstInt("SummonCardID");
                if (summonCardID != (int) AllCards.EmptyCardTypes.NoCard)
                {
                    player.BattleGroundManager.AddMech((CardInfo_Mech) AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")));
                }
            }
        }

        public SideEffectValue_ConstInt GetCardIDSideEffectValue()
        {
            return (SideEffectValue_ConstInt) M_SideEffectParam.GetParam("SummonCardID");
        }
    }
}