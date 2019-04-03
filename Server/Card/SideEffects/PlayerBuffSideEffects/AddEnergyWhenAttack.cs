namespace SideEffects
{
    public class AddEnergyWhenAttack : AddEnergyWhenAttack_Base
    {
        public AddEnergyWhenAttack()
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