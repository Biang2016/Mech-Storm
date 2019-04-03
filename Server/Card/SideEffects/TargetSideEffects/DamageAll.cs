﻿namespace SideEffects
{
    public class DamageAll : DamageAll_Base
    {
        public DamageAll()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.DamageAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValue);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.DamageAllRetinues(FinalValue);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValue);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.DamageAllHeros(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllHeros(FinalValue);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.DamageAllHeros(FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllHeros(FinalValue);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.DamageAllSoldiers(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllSoldiers(FinalValue);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.DamageAllSoldiers(FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllSoldiers(FinalValue);
                    break;
                case TargetRange.Ships:
                    player.DamageLifeAboveZero(FinalValue);
                    player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    break;
                case TargetRange.SelfShip:
                    player.DamageLifeAboveZero(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    break;
                case TargetRange.AllLife:
                    player.DamageLifeAboveZero(FinalValue);
                    player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
                    player.MyBattleGroundManager.DamageAllRetinues(FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValue);
                    break;
            }
        }
    }
}