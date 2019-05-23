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
                player.MyGameManager.GetMech(executorInfo.MechId).M_MechArmor += value;
            }
            else
            {
                player.MyGameManager.SideEffect_MechAction(
                    delegate(ServerModuleMech mech) { mech.M_MechArmor += value; },
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