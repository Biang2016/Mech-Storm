using System;

namespace SideEffects
{
    public class AddLifeAll : AddLifeAll_Base
    {
        public AddLifeAll()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(Value);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(Value);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(Value);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.AddLifeForAllHeros(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(Value);
                    break;
                case TargetRange.SelfHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(Value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllHeros(Value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.AddLifeForAllSoldiers(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(Value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(Value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllSoldiers(Value);
                    break;
                case TargetRange.Ships:
                    player.AddLifeWithinMax(Value);
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    break;
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(Value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    break;
                case TargetRange.All:
                    player.MyBattleGroundManager.AddLifeForAllRetinues(Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForAllRetinues(Value);
                    player.AddLifeWithinMax(Value);
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);

                    break;
            }
        }
    }
}