﻿using System;

namespace SideEffects
{
    public class DamageRandom : DamageRandom_Base
    {
        public DamageRandom()
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
                        player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.DamageRandomRetinue(FinalValue, retinueId);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomRetinue(FinalValue, retinueId);
                    break;
                case TargetRange.Heros:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.DamageRandomHero(FinalValue, retinueId);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomHero(FinalValue, retinueId);
                    break;
                case TargetRange.Soldiers:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Soldier, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageRandomSoldier(FinalValue, retinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomSoldier(FinalValue, retinueId);
                    break;
                case TargetRange.Ships:
                {
                    Random rd = new Random();
                    if (rd.Next(0, 2) == 1)
                    {
                        player.DamageLifeAboveZero(FinalValue);
                    }
                    else
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    }

                    break;
                }
                case TargetRange.All:
                {
                    int retinueNum = player.MyGameManager.CountAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);

                    Random rd = new Random();
                    int ranResult = rd.Next(0, retinueNum + 2);
                    if (ranResult < retinueNum)
                    {
                        ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                        if (retinue != null)
                        {
                            player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                            player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, FinalValue);
                        }
                    }
                    else if (ranResult == retinueNum)
                    {
                        player.DamageLifeAboveZero(FinalValue);
                    }
                    else
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    }

                    break;
                }
            }
        }
    }
}