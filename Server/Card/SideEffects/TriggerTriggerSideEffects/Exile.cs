namespace SideEffects
{
    public class Exile : Exile_Base
    {
        public Exile()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer)Player;
            player.MyHandManager.GetCardByCardInstanceId(executerInfo.CardInstanceId).CardInfo.BaseInfo.IsTemp = true;
        }
    }
}