using System;

namespace SideEffects
{
    public class AddArmor : AddArmor_Base
    {
        public AddArmor()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ArmorValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyGameManager.GetRetinue(executorInfo.RetinueId).M_RetinueArmor += value;
            }
            else
            {
                player.MyGameManager.SideEffect_RetinueAction(
                    delegate(ServerModuleRetinue retinue) { retinue.M_RetinueArmor += value; },
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