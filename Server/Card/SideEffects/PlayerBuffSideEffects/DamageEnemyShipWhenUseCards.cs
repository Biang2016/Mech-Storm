namespace SideEffects
{
    public class DamageEnemyShipWhenUseCards : DamageEnemyShipWhenUseCards_Base
    {
        public DamageEnemyShipWhenUseCards()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.PlayerBuffTrigger(M_ExecuterInfo.SideEffectExecutorID, this);
            foreach (SideEffectBase se in Sub_SideEffect)
            {
                se.Player = player;
                se.Execute(executerInfo);
            }
        }
    }
}