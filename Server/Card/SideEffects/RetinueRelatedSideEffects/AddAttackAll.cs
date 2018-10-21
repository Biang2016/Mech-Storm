namespace SideEffects
{
    public class AddAttackAll : AddAttackAll_Base
    {
        public AddAttackAll()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.AddAttackForAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllRetinues(FinalValue);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.AddAttackForAllRetinues(FinalValue);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllRetinues(FinalValue);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.AddAttackForAllHeros(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllHeros(FinalValue);
                    break;
                case TargetRange.SelfHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllHeros(FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllHeros(FinalValue);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.AddAttackForAllSoldiers(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllSoldiers(FinalValue);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllSoldiers(FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllSoldiers(FinalValue);
                    break;
                case TargetRange.AllLife:
                    player.MyBattleGroundManager.AddAttackForAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllRetinues(FinalValue);

                    break;
            }
        }
    }
}