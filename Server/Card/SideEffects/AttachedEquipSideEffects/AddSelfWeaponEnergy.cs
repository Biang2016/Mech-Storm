﻿using System;

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

            ServerModuleMech mech = player.MyGameManager.GetMechOnBattleGround(M_ExecutorInfo.MechId);
            if (mech?.M_Weapon != null)
            {
                int increase = Math.Min(mech.M_MechWeaponEnergyMax - mech.M_MechWeaponEnergy, M_SideEffectParam.GetParam_MultipliedInt("Energy"));
                mech.M_MechWeaponEnergy += increase;
            }
        }
    }
}