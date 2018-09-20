namespace SideEffects
{
    public class AddLifeAll : AddLifeAll_Base
    {
        public AddLifeAll()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.AddLifeForAllHeros(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(FinalValue);
                    break;
                case TargetRange.SelfHeros:
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
                case TargetRange.All:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(FinalValue);
                    player.AddLifeWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);

                    break;
            }
        }
    }
}