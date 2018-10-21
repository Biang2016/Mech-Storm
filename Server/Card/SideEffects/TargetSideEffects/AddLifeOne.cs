namespace SideEffects
{
    public class AddLifeOne : AddLifeOne_Base
    {
        public AddLifeOne()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddLifeForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            }

            if ((M_TargetRange & TargetRange.Ships) != 0)
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
                    if ((M_TargetRange & TargetRange.SelfShip) != 0) player.AddLifeWithinMax(FinalValue);
                    if ((M_TargetRange & TargetRange.EnemyShip) != 0) player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                }
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddLifeForOneRetinue(executerInfo.RetinueId, FinalValue);
            }
        }
    }
}