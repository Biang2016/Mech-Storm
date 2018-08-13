using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideEffects
{
    public class DamageRandomRetinue : DamageRandomRetinue_Base
    {
        public DamageRandomRetinue()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.SelfBattleGround:
                    DoDamageRandomRetinue(player);
                    break;
                case TargetRange.EnemyBattleGround:
                    DoDamageRandomRetinue(player.MyEnemyPlayer);
                    break;
                case TargetRange.SelfHeros:
                    DoDamageRandomRetinue(player);
                    break;
                case TargetRange.EnemyHeros:
                    DoDamageRandomRetinue(player.MyEnemyPlayer);
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
                        DoDamageRandomRetinue(player);
                        DoDamageRandomRetinue(player.MyEnemyPlayer);
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

        private void DoDamageRandomRetinue(ServerPlayer player)
        {
            player.MyBattleGroundManager.DamageRandomRetinue(Value);
        }

        private void DoDamageShip(ServerPlayer player)
        {
        }
    }
}