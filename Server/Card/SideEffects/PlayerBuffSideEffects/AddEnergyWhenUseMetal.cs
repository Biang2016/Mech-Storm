namespace SideEffects
{
    public class AddEnergyWhenUseMetal : AddEnergyWhenUseMetal_Base
    {
        public AddEnergyWhenUseMetal()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.PlayerBuffTrigger(M_ExecuterInfo.SideEffectExecutorID, this);
            foreach (SideEffectBase se in Sub_SideEffect)
            {
                se.Player = player;
                ((AddEnergy) se).SetFactor(executerInfo.Value);
                se.Execute(executerInfo);
            }
        }
    }
}