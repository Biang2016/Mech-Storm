namespace SideEffects
{
    public class DamageSomeRetinue : DamageSomeRetinue_Base
    {
        public DamageSomeRetinue()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.SelfBattleGround:
                    DoDamageRetinue(player);
                    break;
                case TargetRange.EnemyBattleGround:
                    DoDamageRetinue(player.MyEnemyPlayer);
                    break;
                case TargetRange.SelfHeros:
                    DoDamageRetinue(player);
                    break;
                case TargetRange.EnemyHeros:
                    DoDamageRetinue(player.MyEnemyPlayer);
                    break;
                case TargetRange.SelfShip:
                    DoDamageShip(player);
                    break;
                case TargetRange.EnemyShip:
                    DoDamageShip(player.MyEnemyPlayer);
                    break;
                case TargetRange.All:
                    if (TargetRetinueId >= 0) //随从
                    {
                        DoDamageRetinue(player);
                        DoDamageRetinue(player.MyEnemyPlayer);
                    }
                    else if (TargetRetinueId == -1) //SelfShip
                    {
                        DoDamageShip(player);
                    }
                    else if (TargetRetinueId == -2) //EnemyShip
                    {
                        DoDamageShip(player.MyEnemyPlayer);
                    }

                    break;
            }
        }

        private void DoDamageRetinue(ServerPlayer player)
        {
            player.MyBattleGroundManager.DamageSomeRetinue(TargetRetinueId, Value);
        }

        private void DoDamageShip(ServerPlayer player)
        {
        }
    }
}