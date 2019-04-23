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
            int value = M_SideEffectParam.GetParam_MultipliedInt("Damage");

            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.DamageOneRetinue(executorInfo.TargetRetinueId, value);
                player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(executorInfo.TargetRetinueId, value);
            }

            if ((TargetRange & TargetRange.Ships) != 0)
            {
                if (executorInfo.TargetClientId == player.ClientId)
                {
                    player.DamageLifeAboveZero(value);
                }
                else if (executorInfo.TargetClientId == player.EnemyClientId)
                {
                    player.MyEnemyPlayer.DamageLifeAboveZero(value);
                }
                else
                {
                    if ((TargetRange & TargetRange.SelfShip) != 0) player.DamageLifeAboveZero(value);
                    if ((TargetRange & TargetRange.EnemyShip) != 0) player.MyEnemyPlayer.DamageLifeAboveZero(value);
                }
            }

            if (TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.DamageOneRetinue(executorInfo.RetinueId, value);
            }
        }
    }
}