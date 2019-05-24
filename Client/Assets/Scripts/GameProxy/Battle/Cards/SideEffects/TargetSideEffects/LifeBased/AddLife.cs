namespace SideEffects
{
    public class AddLife : AddLife_Base
    {
        public AddLife()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.BattleGroundManager.GetMech(executorInfo.MechId).AddLife(value);
            }
            else
            {
                player.GameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.Damage(value); },
                    player,
                    ChoiceCount,
                    TargetRange,
                    TargetSelect,
                    executorInfo.TargetClientIds,
                    executorInfo.TargetMechIds
                );
            }
        }
    }
}