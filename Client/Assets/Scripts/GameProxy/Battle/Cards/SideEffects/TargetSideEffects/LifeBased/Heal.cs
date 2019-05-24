namespace SideEffects
{
    public class Heal : Heal_Base
    {
        public Heal()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("HealValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.BattleGroundManager.GetMech(executorInfo.MechId).Heal(value);
            }
            else
            {
                player.GameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.Heal(value); },
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