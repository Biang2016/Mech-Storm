using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AddLifeForSomeRetinue : AddLifeForSomeRetinue_Base
{
    public AddLifeForSomeRetinue()
    {

    }

    public override string GenerateDesc()
    {
        return String.Format(DescRaw, Info.RetinuePlayer, Info.Select, Info.Value);
    }

    public override void Excute(object Player)
    {
        ClientPlayer player = (ClientPlayer)Player;
        switch (Info.RetinuePlayer)
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