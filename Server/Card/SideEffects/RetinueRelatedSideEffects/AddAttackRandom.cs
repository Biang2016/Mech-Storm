namespace SideEffects
{
    public class AddAttackRandom : AddAttackRandom_Base
    {
        public AddAttackRandom()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int retinueId = executerInfo.RetinueId;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.AddAttackForRandomRetinue(FinalValue, retinueId);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForRandomRetinue(FinalValue, retinueId);
                    break;
                case TargetRange.Heroes:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.AddAttackForRandomHero(FinalValue, retinueId);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForRandomHero(FinalValue, retinueId);
                    break;
                case TargetRange.Soldiers:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Soldier, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.AddAttackForRandomSoldier(FinalValue, retinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForRandomSoldier(FinalValue, retinueId);
                    break;
                case TargetRange.AllLife:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
            }
        }
    }
}