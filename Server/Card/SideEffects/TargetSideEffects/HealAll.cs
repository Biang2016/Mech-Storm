namespace SideEffects
{
    public class HealAll : HealAll_Base
    {
        public HealAll()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("HealValue");
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.HealAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(value);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.HealAllRetinues(value);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(value);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.HealAllHeros(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(value);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.HealAllHeros(value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllHeros(value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.HealAllSoldiers(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllSoldiers(value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.HealAllSoldiers(value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllSoldiers(value);
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
                    player.MyBattleGroundManager.HealAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealAllRetinues(value);
                    player.AddLifeWithinMax(value);
                    player.MyEnemyPlayer.AddLifeWithinMax(value);
                    break;
            }
        }
    }
}