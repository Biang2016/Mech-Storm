namespace SideEffects
{
    public class HalfShield : HalfShield_Base
    {
        public HalfShield()
        {
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