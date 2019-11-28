using System.Collections.Generic;

namespace SideEffects
{
    public class TriggerHandCard : HandCardRelatedSideEffect, IPriorUsed, IPositive
    {
        public TriggerHandCard()
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
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            List<int> cardInstanceIds = player.HandManager.GetRandomSpellCardInstanceIds(M_SideEffectParam.GetParam_MultipliedInt("CardCount"), executorInfo.CardInstanceId);
            foreach (int cardInstanceId in cardInstanceIds)
            {
                player.HandManager.UseCard(cardInstanceId, onlyTriggerNotUse: true);
            }
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("CardCount") * 5;
        }
    }
}