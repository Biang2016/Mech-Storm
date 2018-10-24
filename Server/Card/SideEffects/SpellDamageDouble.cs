namespace SideEffects
{
    public class DoubleEnergy : DoubleEnergy_Base
    {
        public DoubleEnergy()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.AddEnergyWithinMax(player.EnergyLeft);
        }
    }
}