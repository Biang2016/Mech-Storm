using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

internal class KillAllInBattleGround : KillAllInBattleGround_Base
{
    public KillAllInBattleGround()
    {
    }

    public override String GenerateDesc()
    {
        return String.Format(DescRaw, GetChineseDescOfTargetRange(M_TargetRange));
    }

    public override void Excute(object Player)
    {
    }
}