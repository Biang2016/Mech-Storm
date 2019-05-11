namespace SideEffects
{
    public class SummonMechByMechCount : SummonMechByMechCount_Base
    {
        public SummonMechByMechCount()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = player.MyGameManager.CountMechsByTargetRange(TargetRange, player);

            for (int i = 0; i < value; i++)
            {
                player.MyBattleGroundManager.AddMech((CardInfo_Mech) AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")));
            }
        }
    }
}