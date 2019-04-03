namespace SideEffects
{
    public class AddTempCardToDeck : AddTempCardToDeck_Base
    {
        public AddTempCardToDeck()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyCardDeckManager.RandomInsertTempCard(CardId);
        }
    }
}