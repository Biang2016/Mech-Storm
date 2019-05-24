namespace SideEffects
{
    public class AddTempCardToEnemyDeck : AddTempCardToEnemyDeck_Base
    {
        public AddTempCardToEnemyDeck()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.MyEnemyPlayer.CardDeckManager.RandomInsertTempCard(M_SideEffectParam.GetParam_ConstInt("CardID"), M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }
    }
}