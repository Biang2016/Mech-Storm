using System;

namespace SideEffects
{
    public class AddWeaponEnergy : AddWeaponEnergy_Base
    {
        public AddWeaponEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            ServerModuleRetinue retinue = player.MyGameManager.GetRetinueOnBattleGround(RetinueID);
            int value = M_SideEffectParam.GetParam_MultipliedInt("EnergyValue");
            if (retinue?.M_Weapon != null)
            {
                int increase = Math.Min(retinue.M_RetinueWeaponEnergyMax - retinue.M_RetinueWeaponEnergy, value);
                retinue.M_RetinueWeaponEnergy += increase;
            }
        }
    }
}