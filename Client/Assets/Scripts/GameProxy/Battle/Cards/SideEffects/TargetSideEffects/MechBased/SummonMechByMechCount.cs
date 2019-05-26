using System.Collections.Generic;

namespace SideEffects
{
    public class SummonMechByMechCount : TargetSideEffect
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
            BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")).BaseInfo;
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
                player.BattleGroundManager.AddMech((CardInfo_Mech) AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")));
            }
        }
    }
}