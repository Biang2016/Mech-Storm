using System;

namespace SideEffects
{
    public class DamageRandom : DamageRandom_Base
    {
        public DamageRandom()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.RetinueCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.DamageRandomRetinue(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomRetinue(Value);
                    }

                    break;
                }
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.DamageRandomRetinue(Value);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomRetinue(Value);
                    break;
                case TargetRange.Heros:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.HeroCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.DamageRandomHero(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomHero(Value);
                    }

                    break;
                }
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.DamageRandomHero(Value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomHero(Value);
                    break;
                case TargetRange.Soldiers:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.SoldierCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.DamageRandomSoldier(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomSoldier(Value);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageRandomSoldier(Value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomSoldier(Value);
                    break;
                case TargetRange.Ships:
                {
                    Random rd = new Random();
                    if (rd.Next(0, 2) == 1)
                    {
                        player.DamageLifeAboveZero(Value);
                    }
                    else
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(Value);
                    }

                    break;
                }
                case TargetRange.All:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.RetinueCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum + 2);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.DamageRandomRetinue(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageRandomRetinue(Value);
                    }
                    else if (ranResult == selfRetinueNum + enemyRetinueNum)
                    {
                        player.DamageLifeAboveZero(Value);
                    }
                    else if (ranResult == selfRetinueNum + enemyRetinueNum + 1)
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(Value);
                    }

                    break;
                }
            }
        }
    }
}