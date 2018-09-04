using System;

namespace SideEffects
{
    public class AddLifeRandom : AddLifeRandom_Base
    {
        public AddLifeRandom()
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
                        player.MyBattleGroundManager.AddLifeForRandomRetinue(FinalValue);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomRetinue(FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.AddLifeForRandomRetinue(FinalValue);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomRetinue(FinalValue);
                    break;
                case TargetRange.Heros:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.HeroCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.AddLifeForRandomHero(FinalValue);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomHero(FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.AddLifeForRandomHero(FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomHero(FinalValue);
                    break;
                case TargetRange.Soldiers:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.SoldierCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.AddLifeForRandomSoldier(FinalValue);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomSoldier(FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.AddLifeForRandomSoldier(FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomSoldier(FinalValue);
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

                case TargetRange.All:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.RetinueCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum + 2);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.AddLifeForRandomRetinue(FinalValue);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomRetinue(FinalValue);
                    }
                    else if (ranResult == selfRetinueNum + enemyRetinueNum)
                    {
                        player.AddLifeWithinMax(FinalValue);
                    }
                    else if (ranResult == selfRetinueNum + enemyRetinueNum + 1)
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }

                    break;
                }
            }
        }
    }
}