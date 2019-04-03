namespace SideEffects
{
    public class DamageOne : DamageOne_Base
    {
        public DamageOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.DamageOneRetinue(executorInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(executorInfo.TargetRetinueId, FinalValue);
            }

            if ((M_TargetRange & TargetRange.Ships) != 0)
            {
                if (executorInfo.TargetClientId == player.ClientId)
                {
                    player.DamageLifeAboveZero(FinalValue);
                }
                else if (executorInfo.TargetClientId == player.EnemyClientId)
                {
                    player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                }
                else
                {
                    if ((M_TargetRange & TargetRange.SelfShip) != 0) player.DamageLifeAboveZero(FinalValue);
                    if ((M_TargetRange & TargetRange.EnemyShip) != 0) player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                }
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.DamageOneRetinue(executorInfo.RetinueId, FinalValue);
            }
        }
    }
}