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
                player.MyGameManager.GetRetinue(executorInfo.RetinueId).M_RetinueShield += value;
            }

            player.MyGameManager.SideEffect_RetinueAction(
                delegate (ServerModuleRetinue retinue) { retinue.M_RetinueShield += value; },
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