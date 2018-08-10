using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class HealSomeRetinue : HealSomeRetinue_Base
{
    public HealSomeRetinue()
    {
    }

    public override string GenerateDesc()
    {
        return String.Format(DescRaw, GetChineseDescOfTargetRange(M_TargetRange), Value);
    }

    public override void Excute(object Player)
    {
        
    }
}