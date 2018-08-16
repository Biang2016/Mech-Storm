namespace SideEffects
{
    public class KillAllInBattleGround : KillAllInBattleGround_Base
    {
        public KillAllInBattleGround()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer)Player;
            switch (M_TargetRange)
            {
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.KillAllInBattleGround();
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.KillAllHerosInBattleGround();
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllHerosInBattleGround();
                    break;
                case TargetRange.SelfSodiers:
                    player.MyBattleGroundManager.KillAllSodiersInBattleGround();
                    break;
                case TargetRange.EnemySodiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllSodiersInBattleGround();
                    break;
                case TargetRange.SelfShip:
                    break;
                case TargetRange.EnemyShip:
                    break;
                case TargetRange.All:
                    player.MyBattleGroundManager.KillAllInBattleGround();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                    break;
            }
        }
    }

}
