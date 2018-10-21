namespace SideEffects
{
    public class KillOne : KillOne_Base
    {
        public KillOne()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.KillOneRetinue(executerInfo.TargetRetinueId);
                player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(executerInfo.TargetRetinueId);
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.KillOneRetinue(executerInfo.RetinueId);
            }
        }
    }
}