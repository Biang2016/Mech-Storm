using System.Collections.Generic;

namespace SideEffects
{
    public class TriggerOneHandCard : TriggerOneHandCard_Base
    {
        public TriggerOneHandCard()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            List<int> cardInstanceIds = player.MyHandManager.GetRandomSpellCardInstanceIds(M_SideEffectParam.GetParam_MultipliedInt("CardCount"), executorInfo.CardInstanceId);
            foreach (int cardInstanceId in cardInstanceIds)
            {
                player.MyHandManager.UseCard(cardInstanceId, onlyTriggerNotUse: true);
            }
        }
    }
}