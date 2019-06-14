using System.Collections.Generic;

namespace SideEffects
{
    public class TriggerHandCard : HandCardRelatedSideEffect
    {
        public TriggerHandCard()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
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
    }
}