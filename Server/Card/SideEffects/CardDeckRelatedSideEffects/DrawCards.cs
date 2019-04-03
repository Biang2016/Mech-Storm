namespace SideEffects
{
    public class DrawCards : DrawCards_Base
    {
        public DrawCards()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyHandManager.DrawCards(FinalValue);
        }
    }
}