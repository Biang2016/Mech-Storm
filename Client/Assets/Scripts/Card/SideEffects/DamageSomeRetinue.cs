using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class DamageSomeRetinue : DamageSomeRetinue_Base
{
    public DamageSomeRetinue()
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