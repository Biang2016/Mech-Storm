using System.Collections.Generic;

namespace SideEffects
{
    public class TriggerOneHandCard : TriggerOneHandCard_Base
    {
        public TriggerOneHandCard()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            List<int> cardInstanceIds = player.MyHandManager.GetRandomSpellCardInstanceIds(FinalValue, executerInfo.CardInstanceId);
            foreach (int cardInstanceId in cardInstanceIds)
            {
                player.MyHandManager.UseCard(cardInstanceId, onlyTriggerNotUse: true);
            }
        }
    }
}