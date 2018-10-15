using System;

namespace SideEffects
{
    public class AddSelfWeaponEnergy : AddSelfWeaponEnergy_Base
    {
        public AddSelfWeaponEnergy()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            ServerModuleRetinue retinue = player.MyGameManager.GetRetinueOnBattleGround(RetinueID);
            if (retinue?.M_Weapon != null)
            {
                int increase = Math.Min(retinue.M_RetinueWeaponEnergyMax - retinue.M_RetinueWeaponEnergy, FinalValue);
                retinue.M_RetinueWeaponEnergy += increase;
            }
        }
    }
}