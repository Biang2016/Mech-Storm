namespace SideEffects
{
    public class AddAttackRandom : AddAttackRandom_Base
    {
        public AddAttackRandom()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int retinueId = executorInfo.RetinueId;
            int value = M_SideEffectParam.GetParam_MultipliedInt("AttackValue");
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.AddAttackForRandomRetinue(value, retinueId);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForRandomRetinue(value, retinueId);
                    break;
                case TargetRange.Heroes:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.AddAttackForRandomHero(value, retinueId);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForRandomHero(value, retinueId);
                    break;
                case TargetRange.Soldiers:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Soldier, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.AddAttackForRandomSoldier(value, retinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForRandomSoldier(value, retinueId);
                    break;
                case TargetRange.AllLife:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
            }
        }
    }
}