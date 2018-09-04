using System;

namespace SideEffects
{
    public class DamageOne : DamageOne_Base
    {
        public DamageOne()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                {
                    if (TargetRetinueId >= 0) //随从
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    }

                    break;
                }
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.Ships:
                    if (TargetRetinueId == -1)
                    {
                        player.DamageLifeAboveZero(FinalValue);
                    }
                    else
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    }

                    break;
                case TargetRange.SelfShip:
                    player.DamageLifeAboveZero(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    break;
                case TargetRange.All:
                    if (TargetRetinueId >= 0) //随从
                    {
                        player.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageOneRetinue(TargetRetinueId, FinalValue);
                    }
                    else if (TargetRetinueId == -1) //SelfShip
                    {
                        player.DamageLifeAboveZero(FinalValue);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    }

                    break;
            }
        }
    }
}