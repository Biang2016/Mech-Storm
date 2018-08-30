﻿namespace SideEffects
{
    public class KillAll : KillAll_Base
    {
        public KillAll()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer)Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.KillAllRetinues();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllRetinues();
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.KillAllRetinues();
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllRetinues();
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.KillAllHeros();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllHeros();
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.KillAllHeros();
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllHeros();
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.KillAllSodiers();
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllSodiers();
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.KillAllSodiers();
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.KillAllSodiers();
                    break;
                case TargetRange.SelfShip:
                    break;
                case TargetRange.EnemyShip:
                    break;
                case TargetRange.All:
                    break;
            }
        }
    }

}