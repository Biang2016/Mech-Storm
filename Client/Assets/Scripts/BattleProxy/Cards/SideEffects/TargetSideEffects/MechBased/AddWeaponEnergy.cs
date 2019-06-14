using System;

namespace SideEffects
{
    public class AddWeaponEnergy : TargetSideEffect
    {
        public AddWeaponEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Energy"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            if (TargetRange == TargetRange.Self)
            {
                ModuleMech mech = player.BattleGroundManager.GetMech(executorInfo.MechId);
                if (mech?.M_Weapon != null)
                {
                    int increase = Math.Min(mech.M_MechWeaponEnergyMax - mech.M_MechWeaponEnergy, value);
                    mech.M_MechWeaponEnergy += increase;
                }
            }
            else
            {
                player.GameManager.SideEffect_MechAction(
                    delegate(ModuleMech mech)
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