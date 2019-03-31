public class DoubleEnergy_Base : SideEffectBase
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}