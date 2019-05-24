namespace SideEffects
{
    public class WaitInHandDecreaseEnergy : WaitInHandDecreaseEnergy_Base
    {
        public WaitInHandDecreaseEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer sp = (BattlePlayer) Player;
            CardBase ci = sp.HandManager.GetCardByCardInstanceId(TargetCardInstanceId);

            if (ci.CardInfo.BaseInfo.Energy >= M_SideEffectParam.GetParam_MultipliedInt("DecValue"))
            {
                ci.M_Energy -= M_SideEffectParam.GetParam_MultipliedInt("DecValue");
            }
        }
    }
}