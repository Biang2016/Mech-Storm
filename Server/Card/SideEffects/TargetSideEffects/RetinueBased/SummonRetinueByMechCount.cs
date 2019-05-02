namespace SideEffects
{
    public class SummonRetinueByMechCount : SummonRetinueByMechCount_Base
    {
        public SummonRetinueByMechCount()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = player.MyGameManager.CountMechsByTargetRange(TargetRange, player);

            for (int i = 0; i < value; i++)
            {
                player.MyBattleGroundManager.AddRetinue((CardInfo_Retinue) AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")));
            }
        }
    }
}