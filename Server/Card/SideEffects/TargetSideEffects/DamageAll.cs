namespace SideEffects
{
    public class DamageAll : DamageAll_Base
    {
        public DamageAll()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Damage");
            switch (TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.DamageAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(value);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.DamageAllRetinues(value);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(value);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.DamageAllHeros(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllHeros(value);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.DamageAllHeros(value);
                    break;
                case TargetRange.EnemyHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllHeros(value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.DamageAllSoldiers(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllSoldiers(value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageAllSoldiers(value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllSoldiers(value);
                    break;
                case TargetRange.Ships:
                    player.DamageLifeAboveZero(value);
                    player.MyEnemyPlayer.DamageLifeAboveZero(value);
                    break;
                case TargetRange.SelfShip:
                    player.DamageLifeAboveZero(value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.DamageLifeAboveZero(value);
                    break;
                case TargetRange.AllLife:
                    player.DamageLifeAboveZero(value);
                    player.MyEnemyPlayer.DamageLifeAboveZero(value);
                    player.MyBattleGroundManager.DamageAllRetinues(value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(value);
                    break;
            }
        }
    }
}