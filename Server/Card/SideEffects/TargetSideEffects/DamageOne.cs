namespace SideEffects
{
    public class DamageOne : DamageOne_Base
    {
        public DamageOne()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if (TR_Retinues.Contains(M_TargetRange))
            {
                player.MyBattleGroundManager.DamageOneRetinue(executerInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            }

            if (TR_Ships.Contains(M_TargetRange))
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
                    if (M_TargetRange == TargetRange.SelfShip) player.DamageLifeAboveZero(FinalValue);
                    if (M_TargetRange == TargetRange.EnemyShip) player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    if (M_TargetRange == TargetRange.All)
                    {
                        player.DamageLifeAboveZero(FinalValue);
                        player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    }
                }
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.DamageOneRetinue(executerInfo.RetinueId, FinalValue);
            }
        }
    }
}