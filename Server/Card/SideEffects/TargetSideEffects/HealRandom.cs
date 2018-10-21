using System;

namespace SideEffects
{
    public class HealRandom : HealRandom_Base
    {
        public HealRandom()
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
                        player.MyBattleGroundManager.HealOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.HealRandomRetinue(FinalValue, retinueId);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealRandomRetinue(FinalValue, retinueId);
                    break;
                case TargetRange.Heros:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.HealOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.HealRandomHero(FinalValue, retinueId);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealRandomHero(FinalValue, retinueId);
                    break;
                case TargetRange.Soldiers:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Soldier, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.HealOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }

                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.HealRandomSoldier(FinalValue, retinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealRandomSoldier(FinalValue, retinueId);
                    break;
                case TargetRange.Ships:
                {
                    Random rd = new Random();
                    if (rd.Next(0, 2) == 1)
                    {
                        player.AddLifeWithinMax(FinalValue);
                    }
                    else
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.AllLife:
                {
                    int retinueNum = player.MyGameManager.CountAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);

                    Random rd = new Random();
                    int ranResult = rd.Next(0, retinueNum + 2);
                    if (ranResult < retinueNum)
                    {
                        ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                        if (retinue != null)
                        {
                            player.MyBattleGroundManager.AddLifeForOneRetinue(retinue.M_RetinueID, FinalValue);
                            player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(retinue.M_RetinueID, FinalValue);
                        }
                    }
                    else if (ranResult == retinueNum)
                    {
                        player.AddLifeWithinMax(FinalValue);
                    }
                    else
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }

                    break;
                }
            }
        }
    }
}