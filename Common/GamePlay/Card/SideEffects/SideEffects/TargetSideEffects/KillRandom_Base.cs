public class KillRandom_Base : TargetSideEffect
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,false, true));
    }

}