namespace SideEffects
{
    public class DamageOne : DamageOne_Base
    {
        public DamageOne()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.DamageOneRetinue(executerInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            }

            if ((M_TargetRange & TargetRange.Ships) != 0)
            {
                if (executerInfo.TargetClientId == player.ClientId)
                {
                    player.DamageLifeAboveZero(FinalValue);
                }
                else if (executerInfo.TargetClientId == player.EnemyClientId)
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
                player.MyBattleGroundManager.DamageOneRetinue(executerInfo.RetinueId, FinalValue);
            }
        }
    }
}