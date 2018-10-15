namespace SideEffects
{
    public class AddTempCardToDeck : AddTempCardToDeck_Base
    {
        public AddTempCardToDeck()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyCardDeckManager.RandomInsertTempCard(CardId);
        }
    }
}