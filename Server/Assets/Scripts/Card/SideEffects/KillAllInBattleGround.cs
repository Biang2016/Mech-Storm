using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class KillAllInBattleGround : KillAllInBattleGround_Base
{
    public KillAllInBattleGround()
    {
    }

    public override SideEffectBase Clone()
    {
        KillAllInBattleGround se = (KillAllInBattleGround)base.Clone();
        se.Info = Info;
        return se;
    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, Info);
    }

    public override void Excute(object Player)
    {
        ServerPlayer player = (ServerPlayer)Player;
        switch (Info.WhoseBattleGround)
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