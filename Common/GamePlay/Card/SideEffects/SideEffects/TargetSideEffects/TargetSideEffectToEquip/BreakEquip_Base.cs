public class BreakEquip_Base : TargetSideEffectEquip
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,false, false));
    }

}