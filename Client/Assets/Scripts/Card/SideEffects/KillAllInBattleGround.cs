using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class KillAllInBattleGround : SideEffectBase
{
    public string WhoseBattleGround;

    public override SideEffectBase Clone()
    {
        KillAllInBattleGround newSE = new KillAllInBattleGround();
        newSE.SideEffectID = SideEffectID;
        newSE.Name = Name;
        newSE.Desc = Desc;
        newSE.WhoseBattleGround = WhoseBattleGround;

        return newSE;
    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, WhoseBattleGround);
    }

    public override void Excute(object Player)
    {
        ClientPlayer player = (ClientPlayer) Player;
        switch (WhoseBattleGround)
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