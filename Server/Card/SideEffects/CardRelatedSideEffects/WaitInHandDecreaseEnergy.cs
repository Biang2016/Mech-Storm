namespace SideEffects
{
    public class WaitInHandDecreaseEnergy : WaitInHandDecreaseEnergy_Base
    {
        public WaitInHandDecreaseEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer sp = (ServerPlayer) Player;
            ServerCardBase ci = sp.MyHandManager.GetCardByCardInstanceId(TargetCardInstanceId);

            if (ci.CardInfo.BaseInfo.Energy >= FinalValue)
            {
                ci.M_Energy -= FinalValue;
            }
        }
    }
}