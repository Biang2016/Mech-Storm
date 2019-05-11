namespace SideEffects
{
    public class AddAttack : AddAttack_Base
    {
        public AddAttack()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("AttackValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyGameManager.GetMech(executorInfo.MechId).M_MechAttack += value;
            }
            else
            {
                player.MyGameManager.SideEffect_MechAction(
                    delegate(ServerModuleMech mech) { mech.M_MechAttack += value; },
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
}