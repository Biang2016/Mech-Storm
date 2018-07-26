using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForSomeRetinue : AddLifeForSomeRetinue_Base
{
    public AddLifeForSomeRetinue()
    {

    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, Info.RetinuePlayer, Info.Select, Info.Value);
    }

    public override void Excute(object Player)
    {
        ClientPlayer player = (ClientPlayer)Player;
        switch (Info.RetinuePlayer)
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