namespace SideEffects
{
    public class GetHandCardCopy : GetHandCardCopy_Base
    {
        public GetHandCardCopy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            int cardId = player.MyHandManager.GetRandomHandCardId();
            player.MyHandManager.GetATempCardByID(cardId);
        }
    }
}