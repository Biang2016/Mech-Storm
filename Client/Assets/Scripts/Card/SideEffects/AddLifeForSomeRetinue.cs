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
        ClientPlayer player = (ClientPlayer) Player;
        switch (RetinuePlayer)
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