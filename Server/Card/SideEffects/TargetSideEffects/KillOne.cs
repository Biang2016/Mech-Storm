namespace SideEffects
{
    public class KillOne : KillOne_Base
    {
        public KillOne()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if (TR_Retinues.Contains(M_TargetRange))
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