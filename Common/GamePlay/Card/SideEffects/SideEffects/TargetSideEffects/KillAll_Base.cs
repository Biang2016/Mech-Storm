public class KillAll_Base : TargetSideEffect
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,true, false));
    }

}