using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class KillAllInBattleGround : SideEffectBase
{
    public string WhoseBattleGround;

    public override SideEffectBase Clone()
    {
        KillAllInBattleGround newKillAllInBattleGround = new KillAllInBattleGround();
        newKillAllInBattleGround.SideEffectID = SideEffectID;
        newKillAllInBattleGround.Name = Name;
        newKillAllInBattleGround.Desc = Desc;
        newKillAllInBattleGround.WhoseBattleGround = WhoseBattleGround;
        return newKillAllInBattleGround;
    }

    public override void Excute(object Player)
    {
        base.Excute(Player);
        ClientPlayer player = (ClientPlayer) Player;
        switch (WhoseBattleGround)
        {
            case "self":
                

                break;
            case "enemy":
                

                break;
            case "all":
                

                break;
        }
    }
}