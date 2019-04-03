public class NextSpellDamageDouble_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}