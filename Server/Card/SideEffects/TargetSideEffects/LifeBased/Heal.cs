using System.Collections.Generic;

namespace SideEffects
{
    public class Heal : Heal_Base
    {
        public Heal()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("HealValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId).Heal(value);
            }
            else
            {
                player.MyGameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.Heal(value); },
                    player,
                    ChoiceCount,
                    TargetRange,
                    TargetSelect,
                    executorInfo.TargetClientIds,
                    executorInfo.TargetRetinueIds
                );
            }
        }
    }
}