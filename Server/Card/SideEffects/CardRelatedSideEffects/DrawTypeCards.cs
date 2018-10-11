namespace SideEffects
{
    public class DrawTypeCards : DrawTypeCards_Base
    {
        public DrawTypeCards()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyHandManager.DrawCardsByType(DrawCardType, FinalValue);
        }
    }
}