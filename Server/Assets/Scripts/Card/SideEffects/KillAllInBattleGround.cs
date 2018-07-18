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
        ServerPlayer player = (ServerPlayer) Player;
        switch (WhoseBattleGround)
        {
            case "self":
                player.MyBattleGroundManager.KillAllInBattleGround();
                break;
            case "enemy":
                player.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
            case "all":
                player.MyBattleGroundManager.KillAllInBattleGround();
                player.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
        }
    }
}