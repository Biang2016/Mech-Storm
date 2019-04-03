namespace SideEffects
{
    public class DoubleEnergy : DoubleEnergy_Base
    {
        public DoubleEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.AddEnergyWithinMax(player.EnergyLeft);
        }
    }
}