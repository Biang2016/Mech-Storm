namespace SideEffects
{
    public class SummonRetinue : SummonRetinue_Base
    {
        public SummonRetinue()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyBattleGroundManager.AddRetinue((CardInfo_Retinue) AllCards.GetCard(SummonRetinueID));
        }
    }
}