using System;

namespace SideEffects
{
    public class DamageRandom : DamageRandom_Base
    {
        public DamageRandom()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int retinueId = executorInfo.RetinueId;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Damage");
            switch (TargetRange)
            {
                case TargetRange.Mechs:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.All, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.DamageRandomRetinue(value, retinueId);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomRetinue(value, retinueId);
                    break;
                case TargetRange.Heroes:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Hero, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.DamageRandomHero(value, retinueId);
                    break;
                case TargetRange.EnemyHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomHero(value, retinueId);
                    break;
                case TargetRange.Soldiers:
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType.Soldier, retinueId);
                    if (retinue != null)
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageRandomSoldier(value, retinueId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomSoldier(value, retinueId);
                    break;
                case TargetRange.Ships:
                {
                    Random rd = new Random();
                    if (rd.Next(0, 2) == 1)
                    {
                        player.DamageLifeAboveZero(value);
                    }
                    else
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(value);
                    }

                    break;
                }
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
                            player.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                            player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(retinue.M_RetinueID, value);
                        }
                    }
                    else if (ranResult == retinueNum)
                    {
                        player.DamageLifeAboveZero(value);
                    }
                    else
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(value);
                    }

                    break;
                }
            }
        }
    }
}