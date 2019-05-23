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
            int value = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            if (TargetRange == TargetRange.Self)
            {
                ServerModuleMech mech = player.MyBattleGroundManager.GetMech(executorInfo.MechId);
                if (mech?.M_Weapon != null)
                {
                    int increase = Math.Min(mech.M_MechWeaponEnergyMax - mech.M_MechWeaponEnergy, value);
                    mech.M_MechWeaponEnergy += increase;
                }
            }
            else
            {
                player.MyGameManager.SideEffect_MechAction(
                    delegate(ServerModuleMech mech)
                    {
                        if (mech?.M_Weapon != null)
                        {
                            int increase = Math.Min(mech.M_MechWeaponEnergyMax - mech.M_MechWeaponEnergy, value);
                            mech.M_MechWeaponEnergy += increase;
                        }
                    },
                    player,
                    ChoiceCount,
                    executorInfo.TargetMechIds,
                    TargetRange,
                    TargetSelect,
                    -1);
            }
        }
    }
}