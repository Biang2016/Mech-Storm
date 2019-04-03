namespace SideEffects
{
    public class AddTempCardToEnemyDeck : AddTempCardToEnemyDeck_Base
    {
        public AddTempCardToEnemyDeck()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyEnemyPlayer.MyCardDeckManager.RandomInsertTempCard(CardId);
        }
    }
}