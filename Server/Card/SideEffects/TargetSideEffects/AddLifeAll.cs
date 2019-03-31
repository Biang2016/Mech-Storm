namespace SideEffects
{
    public class AddLifeAll : AddLifeAll_Base
    {
        public AddLifeAll()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.AddLifeForAllHeros(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(FinalValue);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(FinalValue);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.AddLifeForAllSoldiers(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(FinalValue);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(FinalValue);
                    break;
                case TargetRange.Ships:
                    player.AddLifeWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.AllLife:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    player.AddLifeWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);

                    break;
            }
        }
    }
}