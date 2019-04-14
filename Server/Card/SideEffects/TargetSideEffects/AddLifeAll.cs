namespace SideEffects
{
    public class AddLifeAll : AddLifeAll_Base
    {
        public AddLifeAll()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(value);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(value);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(value);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.AddLifeForAllHeros(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(value);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.AddLifeForAllSoldiers(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(value);
                    break;
                case TargetRange.Ships:
                    player.AddLifeWithinMax(value);
                    player.MyEnemyPlayer.AddLifeWithinMax(value);
                    break;
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(value);
                    break;
                case TargetRange.AllLife:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(value);
                    player.AddLifeWithinMax(value);
                    player.MyEnemyPlayer.AddLifeWithinMax(value);

                    break;
            }
        }
    }
}