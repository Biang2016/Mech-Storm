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
        ServerPlayer player = (ServerPlayer) Player;
        switch (WhoseBattleGround)
        {
            case "我方":
                player.MyBattleGroundManager.KillAllInBattleGround();
                break;
            case "敌方":
                player.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
            case "":
                player.MyBattleGroundManager.KillAllInBattleGround();
                player.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
        }
    }
}