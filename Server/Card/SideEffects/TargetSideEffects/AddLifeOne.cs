namespace SideEffects
{
    public class AddLifeOne : AddLifeOne_Base
    {
        public AddLifeOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddLifeForOneRetinue(executorInfo.TargetRetinueId, value);
                player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(executorInfo.TargetRetinueId, value);
            }

            if ((M_TargetRange & TargetRange.Ships) != 0)
            {
                if (executorInfo.TargetClientId == player.ClientId)
                {
                    player.AddLifeWithinMax(value);
                }
                else if (executorInfo.TargetClientId == player.EnemyClientId)
                {
                    player.MyEnemyPlayer.AddLifeWithinMax(value);
                }
                else
                {
                    if ((M_TargetRange & TargetRange.SelfShip) != 0) player.AddLifeWithinMax(value);
                    if ((M_TargetRange & TargetRange.EnemyShip) != 0) player.MyEnemyPlayer.AddLifeWithinMax(value);
                }
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddLifeForOneRetinue(executorInfo.RetinueId, value);
            }
        }
    }
}