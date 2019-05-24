namespace SideEffects
{
    public class DamageEnemyShipWhenUseCards : DamageEnemyShipWhenUseCards_Base
    {
        public DamageEnemyShipWhenUseCards()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.PlayerBuffTrigger(M_ExecutorInfo.SideEffectExecutorID, this);
            foreach (SideEffectBase se in Sub_SideEffect)
            {
                se.Player = player;
                se.Execute(executorInfo);
            }
        }
    }
}