using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForSomeRetinue : AddLifeForSomeRetinue_Base
{
    public AddLifeForSomeRetinue()
    {
    }

    public override string GenerateDesc()
    {
        return String.Format(DescRaw, GetChineseDescOfTargetRange(M_TargetRange), Value);
    }

    public override void Excute(object Player)
    {
        ServerPlayer player = (ServerPlayer) Player;
        switch (M_TargetRange)
        {
            case TargetRange.SelfBattleGround:
                DoAddLifeForRetinue(player);
                break;
            case TargetRange.EnemyBattleGround:
                DoAddLifeForRetinue(player.MyEnemyPlayer);
                break;
            case TargetRange.SelfHeros:
                DoAddLifeForRetinue(player);
                break;
            case TargetRange.EnemyHeros:
                DoAddLifeForRetinue(player.MyEnemyPlayer);
                break;
            case TargetRange.SelfShip:
                DoAddShipLife(player);
                break;
            case TargetRange.EnemyShip:
                DoAddShipLife(player.MyEnemyPlayer);
                break;
            case TargetRange.All:
                if (TargetRetinueId >= 0) //随从
                {
                    DoAddLifeForRetinue(player);
                    DoAddLifeForRetinue(player.MyEnemyPlayer);
                }
                else if (TargetRetinueId == -1) //SelfShip
                {
                    DoAddShipLife(player);
                }
                else if (TargetRetinueId == -2) //EnemyShip
                {
                    DoAddShipLife(player.MyEnemyPlayer);
                }

                break;
        }
    }

    private void DoAddLifeForRetinue(ServerPlayer player)
    {
        player.MyBattleGroundManager.AddLifeForSomeRetinue(TargetRetinueId, Value);
    }

    private void DoAddShipLife(ServerPlayer player)
    {
    }
}