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
                        player.MyBattleGroundManager.AddLifeForRandomRetinue(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomRetinue(Value);
                    }

                    break;
                }
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.AddLifeForRandomRetinue(Value);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomRetinue(Value);
                    break;
                case TargetRange.Heros:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.HeroCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.AddLifeForRandomHero(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomHero(Value);
                    }

                    break;
                }
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.AddLifeForRandomHero(Value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomHero(Value);
                    break;
                case TargetRange.Soldiers:
                {
                    int selfRetinueNum = player.MyBattleGroundManager.SoldierCount;
                    int enemyRetinueNum = player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                    Random rd = new Random();
                    int ranResult = rd.Next(0, selfRetinueNum + enemyRetinueNum);
                    if (ranResult < selfRetinueNum)
                    {
                        player.MyBattleGroundManager.AddLifeForRandomSoldier(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomSoldier(Value);
                    }

                    break;
                }
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.AddLifeForRandomSoldier(Value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomSoldier(Value);
                    break;
                case TargetRange.Ships:
                {
                    Random rd = new Random();
                    if (rd.Next(0, 2) == 1)
                    {
                        player.AddLifeWithinMax(Value);
                    }
                    else
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(Value);
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
                        player.MyBattleGroundManager.AddLifeForRandomRetinue(Value);
                    }
                    else if (ranResult < selfRetinueNum + enemyRetinueNum)
                    {
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForRandomRetinue(Value);
                    }
                    else if (ranResult == selfRetinueNum + enemyRetinueNum)
                    {
                        player.AddLifeWithinMax(Value);
                    }
                    else if (ranResult == selfRetinueNum + enemyRetinueNum + 1)
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    }

                    break;
                }
            }
        }
    }
}