using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

internal class KillAllInBattleGround : KillAllInBattleGround_Base
{
    public KillAllInBattleGround()
    {
    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, Info.WhoseBattleGround);
    }

    public override void Excute(object Player)
    {
        ClientPlayer player = (ClientPlayer)Player;
        switch (Info.WhoseBattleGround)
        {
            case "我方":


                break;
            case "敌方":


                break;
            case "":


                break;
        }
    }
}