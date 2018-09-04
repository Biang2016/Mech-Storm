namespace SideEffects
{
    public class HealOne : HealOne_Base
    {
        public HealOne()
        {
        }


        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.BattleGrounds:
                    player.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.SelfBattleGround:
                    player.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.EnemyBattleGround:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.Heros:
                    player.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.SelfHeros:
                    player.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.HealOneRetinue(TargetRetinueId, FinalValue);
                    break;
                case TargetRange.Ships:
                    if (TargetRetinueId == -1) //SelfShip
                    {
                        player.AddLifeWithinMax(FinalValue);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }

                    break;
                case TargetRange.SelfShip:
                    player.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    break;
                case TargetRange.All:
                    if (TargetRetinueId >= 0) //随从
                    {
                        player.AddLifeWithinMax(FinalValue);
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }
                    else if (TargetRetinueId == -1) //SelfShip
                    {
                        player.AddLifeWithinMax(FinalValue);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        player.MyEnemyPlayer.AddLifeWithinMax(FinalValue);
                    }

                    break;
            }
        }
    }
}