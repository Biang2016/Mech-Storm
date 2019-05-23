namespace SideEffects
{
    public class HalfShield : HalfShield_Base
    {
        public HalfShield()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                foreach (int mechId in executorInfo.TargetMechIds)
                {
                    ServerModuleMech mech = player.MyGameManager.GetMechOnBattleGround(mechId);
                    if (mech != null && mech.M_MechShield != 0)
                    {
                        mech.M_MechShield = mech.M_MechShield / 2;
                    }
                }
            }
        }
    }
}