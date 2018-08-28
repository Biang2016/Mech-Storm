namespace SideEffects
{
    public class KillOne : KillOne_Base
    {
        public KillOne()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(TargetRetinueId);
                    break;
                case TargetRange.SelfShip:
                    break;
                case TargetRange.EnemyShip:
                    break;
                case TargetRange.All:
                    break;
            }
        }
    }
}