using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForSomeRetinue : SideEffectBase
{
    public string RetinuePlayer;
    public int RetinuePlaceIndex;
    public int Value;


    public override SideEffectBase Clone()
    {
        AddLifeForSomeRetinue newAddLifeForSomeRetinue = new AddLifeForSomeRetinue();
        newAddLifeForSomeRetinue.SideEffectID = SideEffectID;
        newAddLifeForSomeRetinue.Name = Name;
        newAddLifeForSomeRetinue.Desc = Desc;
        newAddLifeForSomeRetinue.RetinuePlayer = RetinuePlayer;
        newAddLifeForSomeRetinue.RetinuePlaceIndex = RetinuePlaceIndex;
        newAddLifeForSomeRetinue.Value = Value;
        return newAddLifeForSomeRetinue;
    }

    public override void Excute(object Player)
    {
        base.Excute(Player);
        ServerPlayer player = (ServerPlayer) Player;
        switch (RetinuePlayer)
        {
            case "self":
                player.MyBattleGroundManager.GetRetinue(RetinuePlaceIndex).M_RetinueLeftLife += Value;
                break;
            case "enemy":
                player.MyEnemyPlayer.MyBattleGroundManager.GetRetinue(RetinuePlaceIndex).M_RetinueLeftLife += Value;
                break;
            case "all":
                player.MyBattleGroundManager.GetRetinue(RetinuePlaceIndex).M_RetinueLeftLife += Value;
                player.MyEnemyPlayer.MyBattleGroundManager.GetRetinue(RetinuePlaceIndex).M_RetinueLeftLife += Value;
                break;
        }
    }
}