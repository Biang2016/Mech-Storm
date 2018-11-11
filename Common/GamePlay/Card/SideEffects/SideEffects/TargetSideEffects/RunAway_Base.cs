using System.Collections.Generic;

public class RunAway_Base : TargetSideEffect
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false));
    }
}