using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForSomeRetinue : AddLifeForSomeRetinue_Base
{
    public AddLifeForSomeRetinue()
    {

    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, Info.RetinuePlayer, Info.Select, Info.Value);
    }

    public override void Excute(object Player)
    {
        ServerPlayer player = (ServerPlayer)Player;
        switch (Info.RetinuePlayer)
        {
            case "我方":
                DoAddLife(player);
                break;
            case "敌方":
                DoAddLife(player.MyEnemyPlayer);
                break;
            case "":
                DoAddLife(player);
                DoAddLife(player.MyEnemyPlayer);
                break;
        }
    }

    private void DoAddLife(ServerPlayer player)
    {
        ServerModuleRetinue retinue;
        switch (Info.Select)
        {
            case "随机":
                retinue = player.MyBattleGroundManager.GetRandomRetinue();
                retinue.M_RetinueLeftLife += Info.Value;
                break;
            case "指定":
                //retinue = player.MyBattleGroundManager.GetRandomRetinue();
                //retinue.M_RetinueLeftLife += Value;
                break;
        }
    }
}