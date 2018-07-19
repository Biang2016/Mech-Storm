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
        ClientPlayer player = (ClientPlayer) Player;
        switch (RetinuePlayer)
        {
            case "我方":
                

                break;
            case "地方":
                

                break;
            case "":
                

                break;
        }
    }
}