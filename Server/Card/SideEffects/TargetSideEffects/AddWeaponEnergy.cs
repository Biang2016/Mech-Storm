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
                ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId);
                if (retinue?.M_Weapon != null)
                {
                    int increase = Math.Min(retinue.M_RetinueWeaponEnergyMax - retinue.M_RetinueWeaponEnergy, value);
                    retinue.M_RetinueWeaponEnergy += increase;
                }
            }
            else
            {
                ServerBattleGroundManager.RetinueType rt = ServerBattleGroundManager.GetRetinueTypeByTargetRange(TargetRange);
                foreach (ServerPlayer p in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
                {
                    p.MyBattleGroundManager.ChangeRetinuesValue(ServerBattleGroundManager.RetinueValueTypes.WeaponEnergy, value, ChoiceCount, executorInfo.TargetRetinueIds, TargetSelect, rt);
                }
            }
        }
    }
}