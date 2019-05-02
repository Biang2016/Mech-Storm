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
                player.MyGameManager.GetRetinue(executorInfo.RetinueId).M_RetinueAttack += value;
            }
            else
            {
                player.MyGameManager.SideEffect_RetinueAction(
                    delegate(ServerModuleRetinue retinue) { retinue.M_RetinueAttack += value; },
                    player,
                    0,
                    executorInfo.TargetRetinueIds,
                    TargetRange,
                    TargetSelect,
                    -1
                );
            }
        }
    }
}