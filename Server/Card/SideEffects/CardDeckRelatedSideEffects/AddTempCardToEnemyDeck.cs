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
            player.MyEnemyPlayer.MyCardDeckManager.RandomInsertTempCard(M_SideEffectParam.GetParam_ConstInt("CardID"), M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }
    }
}