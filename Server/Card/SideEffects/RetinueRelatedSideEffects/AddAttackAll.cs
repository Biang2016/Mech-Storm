namespace SideEffects
{
    public class AddAttackAll : AddAttackAll_Base
    {
        public AddAttackAll()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("AttackValue");
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.AddAttackForAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllRetinues(value);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.AddAttackForAllRetinues(value);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllRetinues(value);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.AddAttackForAllHeros(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllHeros(value);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllHeros(value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllHeros(value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.AddAttackForAllSoldiers(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllSoldiers(value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllSoldiers(value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllSoldiers(value);
                    break;
                case TargetRange.AllLife:
                    player.MyBattleGroundManager.AddAttackForAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForAllRetinues(value);

                    break;
            }
        }
    }
}