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
            BattlePlayer player = (BattlePlayer) Player;
            int energyValue = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            int finalValue = energyValue * player.GameManager.CountMechsByTargetRange(TargetRange, player);
            finalValue = Math.Min(finalValue, player.MyEnemyPlayer.EnergyLeft);
            player.AddEnergy(finalValue);
            player.MyEnemyPlayer.UseEnergy(finalValue);
        }
    }
}