public class BreakEquip_Base : TargetSideEffectEquip
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange((TargetRange) M_SideEffectParam.GetParam_ConstInt("M_TargetRange"), false, false));
    }
}