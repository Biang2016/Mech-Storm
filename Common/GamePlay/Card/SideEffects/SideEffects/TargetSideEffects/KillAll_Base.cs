public class KillAll_Base : TargetSideEffect
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,true, false));
    }

}