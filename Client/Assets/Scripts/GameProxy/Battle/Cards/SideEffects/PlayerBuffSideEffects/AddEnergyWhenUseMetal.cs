namespace SideEffects
{
    public class AddEnergyWhenUseMetal : AddEnergyWhenUseMetal_Base
    {
        public AddEnergyWhenUseMetal()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.PlayerBuffTrigger(M_ExecutorInfo.SideEffectExecutorID, this);
            foreach (SideEffectBase se in Sub_SideEffect)
            {
                se.Player = player;
                ((AddEnergy) se).M_SideEffectParam.Factor = executorInfo.Value;
                se.Execute(executorInfo);
            }
        }
    }
}