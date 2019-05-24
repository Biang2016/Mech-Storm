namespace SideEffects
{
    public class DoubleEnergy : DoubleEnergy_Base
    {
        public DoubleEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.AddEnergy(player.EnergyLeft);
        }
    }
}