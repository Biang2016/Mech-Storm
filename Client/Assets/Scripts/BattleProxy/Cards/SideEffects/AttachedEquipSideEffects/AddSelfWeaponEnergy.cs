using System;

namespace SideEffects
{
    public class AddSelfWeaponEnergy : AttachedEquipSideEffects, IStrengthen, IPriorUsed
    {
        public AddSelfWeaponEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("Energy"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            ModuleMech mech = player.GameManager.GetMechOnBattleGround(M_ExecutorInfo.MechId);
            if (mech?.M_Weapon != null)
            {
                int increase = Math.Min(mech.M_MechWeaponEnergyMax - mech.M_MechWeaponEnergy, M_SideEffectParam.GetParam_MultipliedInt("Energy"));
                mech.M_MechWeaponEnergy += increase;
            }
        }
    }
}