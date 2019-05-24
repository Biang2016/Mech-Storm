using System.Collections.Generic;

namespace SideEffects
{
    public class TriggerHandCard : TriggerHandCard_Base
    {
        public TriggerHandCard()
        {
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