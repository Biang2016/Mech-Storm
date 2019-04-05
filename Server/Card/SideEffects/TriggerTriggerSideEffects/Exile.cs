namespace SideEffects
{
    public class Exile : Exile_Base
    {
        public Exile()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyHandManager.GetCardByCardInstanceId(executorInfo.CardInstanceId).CardInfo.BaseInfo.IsTemp = true;
        }
    }
}