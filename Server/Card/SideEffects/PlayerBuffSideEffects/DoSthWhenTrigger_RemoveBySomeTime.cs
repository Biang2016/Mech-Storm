namespace SideEffects
{
    public class DoSthWhenTrigger_RemoveBySomeTime : DoSthWhenTrigger_RemoveBySomeTime_Base
    {
        public DoSthWhenTrigger_RemoveBySomeTime()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.PlayerBuffTrigger(M_ExecutorInfo.SideEffectExecutorID, this);
            foreach (SideEffectBase se in Sub_SideEffect)
            {
                se.Player = player;
                se.Execute(executorInfo);
            }
        }
    }
}