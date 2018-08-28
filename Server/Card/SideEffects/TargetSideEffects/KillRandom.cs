namespace SideEffects
{
    public class KillRandom : KillRandom_Base
    {
        public KillRandom()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.KillRandomRetinue();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomRetinue();
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.KillRandomRetinue();
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomRetinue();
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.KillRandomHero();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomHero();
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.KillRandomHero();
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomHero();
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.KillRandomSoldier();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomSoldier();
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.KillRandomSoldier();
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomSoldier();
                    break;
                case TargetRange.SelfShip:
                    break;
                case TargetRange.EnemyShip:
                    break;
                case TargetRange.All:
                    break;
            }
        }
    }
}