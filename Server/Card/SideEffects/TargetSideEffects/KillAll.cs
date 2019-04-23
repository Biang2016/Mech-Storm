namespace SideEffects
{
    public class KillAll : KillAll_Base
    {
        public KillAll()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.KillAllRetinues();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllRetinues();
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.KillAllRetinues();
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllRetinues();
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.KillAllHeroes();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllHeroes();
                    break;
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.KillAllHeroes();
                    break;
                case TargetRange.EnemyHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllHeroes();
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.KillAllSoldiers();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllSoldiers();
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.KillAllSoldiers();
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllSoldiers();
                    break;
                case TargetRange.SelfShip:
                    break;
                case TargetRange.EnemyShip:
                    break;
                case TargetRange.AllLife:
                    break;
            }
        }
    }
}