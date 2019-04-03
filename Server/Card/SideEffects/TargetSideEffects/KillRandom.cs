namespace SideEffects
{
    public class KillRandom : KillRandom_Base
    {
        public KillRandom()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int retinueId = executorInfo.RetinueId;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                        player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    }

                    break;
                }

                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.KillRandomRetinue(retinueId);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomRetinue(retinueId);
                    break;
                case TargetRange.Heroes:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                        player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    }

                    break;
                }

                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.KillRandomHero(retinueId);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomHero(retinueId);
                    break;
                case TargetRange.Soldiers:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Soldier, retinueId);
                    player.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.KillRandomSoldier(retinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomSoldier(retinueId);
                    break;
                case TargetRange.SelfShip:
                    break;
                case TargetRange.EnemyShip:
                    break;
                case TargetRange.AllLife:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                        player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    }

                    break;
                }
            }
        }
    }
}