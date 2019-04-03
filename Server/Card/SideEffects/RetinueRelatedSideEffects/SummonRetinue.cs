namespace SideEffects
{
    public class SummonRetinue : SummonRetinue_Base
    {
        public SummonRetinue()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyBattleGroundManager.AddRetinue((CardInfo_Retinue) AllCards.GetCard(SummonRetinueID));
        }
    }
}