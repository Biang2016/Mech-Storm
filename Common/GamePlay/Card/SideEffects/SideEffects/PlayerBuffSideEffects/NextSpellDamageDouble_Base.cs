public class NextSpellDamageDouble_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}