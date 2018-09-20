namespace SideEffects
{
    public class DrawCards : DrawCards_Base
    {
        public DrawCards()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyHandManager.DrawCards(FinalValue);
        }
    }
}