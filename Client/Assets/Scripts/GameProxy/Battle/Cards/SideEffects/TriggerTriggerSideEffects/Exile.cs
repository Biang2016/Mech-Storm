namespace SideEffects
{
    public class Exile : Exile_Base
    {
        public Exile()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.HandManager.GetCardByCardInstanceId(executorInfo.CardInstanceId).CardInfo.BaseInfo.IsTemp = true;
        }
    }
}