namespace SideEffects
{
    public class HealOne : HealOne_Base
    {
        public HealOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.HealOneRetinue(executorInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(executorInfo.TargetRetinueId, FinalValue);
            }

            if ((M_TargetRange & TargetRange.Ships) != 0)
            {
                if (executorInfo.TargetClientId == player.ClientId)
                {
                    player.AddLifeWithinMax(FinalValue);
                }
                else if (executorInfo.TargetClientId == player.EnemyClientId)
                {
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                }
                else
                {
                    if ((M_TargetRange & TargetRange.SelfShip) != 0) player.AddLifeWithinMax(FinalValue);
                    if ((M_TargetRange & TargetRange.EnemyShip) != 0) player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                }
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.HealOneRetinue(executorInfo.RetinueId, FinalValue);
            }
        }
    }
}