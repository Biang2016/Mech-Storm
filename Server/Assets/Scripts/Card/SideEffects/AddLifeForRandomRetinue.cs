using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForRandomRetinue : AddLifeForSomeRetinue_Base
{
    public AddLifeForRandomRetinue()
    {
    }

    public override string GenerateDesc()
    {
        return String.Format(DescRaw, Info.RetinuePlayer, Info.Value);
    }

    public override void Excute(object Player)
    {
        ServerPlayer player = (ServerPlayer) Player;
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
        player.MyBattleGroundManager.AddLifeForRandomRetinue(Info.Value);
    }
}