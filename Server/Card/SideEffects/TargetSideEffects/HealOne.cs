namespace SideEffects
{
    public class HealOne : HealOne_Base
    {
        public HealOne()
        {
        }


        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if (TR_Retinues.Contains(M_TargetRange))
            {
                player.MyBattleGroundManager.HealOneRetinue(executerInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            }

            if (TR_Ships.Contains(M_TargetRange))
            {
                if (executerInfo.TargetClientId == player.ClientId)
                {
                    player.AddLifeWithinMax(FinalValue);
                }
                else if (executerInfo.TargetClientId == player.EnemyClientId)
                {
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                }
                else
                {
                    if (M_TargetRange == TargetRange.SelfShip) player.AddLifeWithinMax(FinalValue);
                    if (M_TargetRange == TargetRange.EnemyShip) player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    if (M_TargetRange == TargetRange.All)
                    {
                        player.AddLifeWithinMax(FinalValue);
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }
                }
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.HealOneRetinue(executerInfo.RetinueId, FinalValue);
            }
        }
    }
}