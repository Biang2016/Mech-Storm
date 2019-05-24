namespace SideEffects
{
    public class GetHandCardCopy : GetHandCardCopy_Base
    {
        public GetHandCardCopy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            int cardId = player.HandManager.GetRandomHandCardId();
            player.HandManager.GetATempCardByID(cardId);
        }
    }
}