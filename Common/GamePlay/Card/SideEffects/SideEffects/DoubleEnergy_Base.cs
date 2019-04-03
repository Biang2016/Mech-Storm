public class DoubleEnergy_Base : SideEffectBase
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}