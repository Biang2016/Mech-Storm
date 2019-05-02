using System;

namespace SideEffects
{
    public class StealEnergyByMechCount : StealEnergyByMechCount_Base
    {
        public StealEnergyByMechCount()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int energyValue = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            int finalValue = energyValue * player.MyGameManager.CountMechsByTargetRange(TargetRange, player);
            finalValue = Math.Min(finalValue, player.MyEnemyPlayer.EnergyLeft);
            player.AddEnergy(finalValue);
            player.MyEnemyPlayer.UseEnergy(finalValue);
        }
    }
}