namespace SideEffects
{
    public class Damage : Damage_Base
    {
        public Damage()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Damage");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId).Damage(value);
            }
            else
            {
                player.MyGameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.Damage(value); },
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