namespace SideEffects
{
    public class HealAll : HealAll_Base
    {
        public HealAll()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.HealAllHeros(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(FinalValue);
                    break;
                case TargetRange.SelfHeros:
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
                case TargetRange.All:
                    player.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(FinalValue);
                    player.AddLifeWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    break;
            }
        }
    }
}