namespace SideEffects
{
    public class HalfShield : TargetSideEffect
    {
        public HalfShield()
        {
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange());
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                foreach (int mechId in executorInfo.TargetMechIds)
                {
                    ModuleMech mech = player.GameManager.GetMechOnBattleGround(mechId);
                    if (mech != null && mech.M_MechShield != 0)
                    {
                        mech.M_MechShield = mech.M_MechShield / 2;
                    }
                }
            }
        }
    }
}