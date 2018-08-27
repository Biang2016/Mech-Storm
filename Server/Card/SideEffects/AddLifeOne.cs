namespace SideEffects
{
    public class AddLifeOne : AddLifeOne_Base
    {
        public AddLifeOne()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.SelfHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    break;
                case TargetRange.Ships:
                    if (TargetRetinueId == -1) //SelfShip
                    {
                        player.AddLifeWithinMax(Value);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    }

                    break;
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(Value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    break;
                case TargetRange.All:
                    if (TargetRetinueId >= 0) //随从
                    {
                        player.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                        player.MyEnemyPlayer.MyBattleGroundManager.AddLifeForOneRetinue(TargetRetinueId, Value);
                    }
                    else if (TargetRetinueId == -1) //SelfShip
                    {
                        player.AddLifeWithinMax(Value);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(Value);
                    }

                    break;
            }
        }
    }
}