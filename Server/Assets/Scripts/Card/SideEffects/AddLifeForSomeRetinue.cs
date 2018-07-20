using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForSomeRetinue : SideEffectBase
{
    public string RetinuePlayer;
    public string Select;
    public int Value;

    public override SideEffectBase Clone()
    {
        AddLifeForSomeRetinue newSE = new AddLifeForSomeRetinue();
        newSE.SideEffectID = SideEffectID;
        newSE.Name = Name;
        newSE.Desc = Desc;
        newSE.RetinuePlayer = RetinuePlayer;
        newSE.Select = Select;
        newSE.Value = Value;

        return newSE;
    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, RetinuePlayer, Select, Value);
    }

    public override void Excute(object Player)
    {
        ServerPlayer player = (ServerPlayer) Player;
        switch (RetinuePlayer)
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
        switch (Select)
        {
            case "随机":
                retinue = player.MyBattleGroundManager.GetRandomRetinue();
                retinue.M_RetinueLeftLife += Value;
                break;
            case "指定":
                //retinue = player.MyBattleGroundManager.GetRandomRetinue();
                //retinue.M_RetinueLeftLife += Value;
                break;
        }
    }
}