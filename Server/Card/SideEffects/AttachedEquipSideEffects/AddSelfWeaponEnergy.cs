using System;

namespace SideEffects
{
    public class AddSelfWeaponEnergy : AddSelfWeaponEnergy_Base
    {
        public AddSelfWeaponEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            ServerModuleRetinue retinue = player.MyGameManager.GetRetinueOnBattleGround(M_ExecutorInfo.TargetRetinueId);
            if (retinue?.M_Weapon != null)
            {
                int increase = Math.Min(retinue.M_RetinueWeaponEnergyMax - retinue.M_RetinueWeaponEnergy, M_SideEffectParam.GetParam_MultipliedInt("Energy"));
                retinue.M_RetinueWeaponEnergy += increase;
            }
        }
    }
}