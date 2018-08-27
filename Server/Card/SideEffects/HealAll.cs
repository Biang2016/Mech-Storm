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
                    player.MyBattleGroundManager.HealAllRetinues(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(Value);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.HealAllRetinues(Value);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(Value);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.HealAllHeros(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(Value);
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.HealAllHeros(Value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(Value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.HealAllSoldiers(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllSoldiers(Value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.HealAllSoldiers(Value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllSoldiers(Value);
                    break;
                case TargetRange.Ships:
                    player.AddLifeWithinMax(Value);
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    break;
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(Value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    break;
                case TargetRange.All:
                    player.MyBattleGroundManager.HealAllRetinues(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(Value);
                    player.AddLifeWithinMax(Value);
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    break;
            }
        }
    }
}