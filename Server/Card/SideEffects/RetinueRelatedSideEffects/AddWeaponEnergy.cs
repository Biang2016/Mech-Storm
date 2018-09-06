using System;

namespace SideEffects
{
    public class AddWeaponEnergy : AddWeaponEnergy_Base
    {
        public AddWeaponEnergy()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(executerInfo.RetinueId);
            if (retinue == null) retinue = player.MyEnemyPlayer.MyBattleGroundManager.GetRetinue(executerInfo.RetinueId);
            if (retinue?.M_Weapon != null)
            {
                int increase = Math.Min(retinue.M_RetinueWeaponEnergyMax - retinue.M_RetinueWeaponEnergy, FinalValue);
                retinue.M_RetinueWeaponEnergy += increase;
            }
        }
    }
}