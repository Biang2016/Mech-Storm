namespace SideEffects
{
    public class HealSomeRetinue : HealSomeRetinue_Base
    {
        public HealSomeRetinue()
        {
        }


        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.SelfBattleGround:
                    DoHealRetinue(player);
                    break;
                case TargetRange.EnemyBattleGround:
                    DoHealRetinue(player.MyEnemyPlayer);
                    break;
                case TargetRange.SelfHeros:
                    DoHealRetinue(player);
                    break;
                case TargetRange.EnemyHeros:
                    DoHealRetinue(player.MyEnemyPlayer);
                    break;
                case TargetRange.SelfShip:
                    DoHealShip(player);
                    break;
                case TargetRange.EnemyShip:
                    DoHealShip(player.MyEnemyPlayer);
                    break;
                case TargetRange.All:
                    if (TargetRetinueId >= 0) //随从
                    {
                        DoHealRetinue(player);
                        DoHealRetinue(player.MyEnemyPlayer);
                    }
                    else if (TargetRetinueId == -1) //SelfShip
                    {
                        DoHealShip(player);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        DoHealShip(player.MyEnemyPlayer);
                    }

                    break;
            }
        }

        private void DoHealRetinue(ServerPlayer player)
        {
            player.MyBattleGroundManager.HealSomeRetinue(TargetRetinueId, Value);
        }

        private void DoHealShip(ServerPlayer player)
        {
        }
    }
}