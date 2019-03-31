namespace SideEffects
{
    public class HealAll : HealAll_Base
    {
        public HealAll()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.HealAllHeros(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(FinalValue);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.HealAllHeros(FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(FinalValue);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.HealAllSoldiers(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllSoldiers(FinalValue);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.HealAllSoldiers(FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllSoldiers(FinalValue);
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
                    player.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    player.AddLifeWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    break;
            }
        }
    }
}