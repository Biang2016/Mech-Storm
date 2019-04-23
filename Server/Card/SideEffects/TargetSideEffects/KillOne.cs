namespace SideEffects
{
    public class KillOne : KillOne_Base
    {
        public KillOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.KillOneRetinue(executorInfo.TargetRetinueId);
                player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(executorInfo.TargetRetinueId);
            }

            if (TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.KillOneRetinue(executorInfo.RetinueId);
            }
        }
    }
}