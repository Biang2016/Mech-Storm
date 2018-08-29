namespace SideEffects
{
    public class DamageAll : DamageAll_Base
    {
        public DamageAll()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.DamageAllRetinues(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(Value);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.DamageAllRetinues(Value);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(Value);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.DamageAllHeros(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllHeros(Value);
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.DamageAllHeros(Value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllHeros(Value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.DamageAllSoldiers(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllSoldiers(Value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageAllSoldiers(Value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllSoldiers(Value);
                    break;
                case TargetRange.Ships:
                    player.DamageLifeAboveZero(Value);
                    player.MyEnemyPlayer.DamageLifeAboveZero(Value);
                    break;
                case TargetRange.SelfShip:
                    player.DamageLifeAboveZero(Value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.DamageLifeAboveZero(Value);
                    break;
                case TargetRange.All:
                    player.DamageLifeAboveZero(Value);
                    player.MyEnemyPlayer.DamageLifeAboveZero(Value);
                    player.MyBattleGroundManager.DamageAllRetinues(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(Value);
                    break;
            }
        }
    }
}