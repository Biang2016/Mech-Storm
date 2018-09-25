using System;

namespace SideEffects
{
    public class KillRandom : KillRandom_Base
    {
        public KillRandom()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int retinueId = executerInfo.RetinueId;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                        player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    }

                    break;
                }

                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.KillRandomRetinue(retinueId);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillRandomRetinue(retinueId);
                    break;
                case TargetRange.Heros:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                        player.MyEnemyPlayer.MyBattleGroundManager.KillOneRetinue(retinue.M_RetinueID);
                    }

                    break;
                }

                case TargetRange.SelfHeros:
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
                case TargetRange.All:
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