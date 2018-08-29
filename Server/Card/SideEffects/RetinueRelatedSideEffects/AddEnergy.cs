using System;

namespace SideEffects
{
    public class AddEnergy : AddEnergy_Base
    {
        public AddEnergy()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(RetinueID);
            if (retinue == null) retinue = player.MyEnemyPlayer.MyBattleGroundManager.GetRetinue(RetinueID);
            if (retinue?.M_Weapon != null)
            {
                int increase = Math.Min(retinue.M_RetinueWeaponEnergyMax - retinue.M_RetinueWeaponEnergy, Value);
                retinue.M_RetinueWeaponEnergy += increase;
            }
        }
    }
}