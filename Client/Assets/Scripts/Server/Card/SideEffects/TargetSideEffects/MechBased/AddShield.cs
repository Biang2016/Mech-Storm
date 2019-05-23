namespace SideEffects
{
    public class AddShield : AddShield_Base
    {
        public AddShield()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ShieldValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyGameManager.GetMech(executorInfo.MechId).M_MechShield += value;
            }

            player.MyGameManager.SideEffect_MechAction(
                delegate (ServerModuleMech mech) { mech.M_MechShield += value; },
                player,
                0,
                executorInfo.TargetMechIds,
                TargetRange,
                TargetSelect,
                -1
            );
        }
    }
}