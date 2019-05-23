public class NextSpellDamageDouble_Base : PlayerBuffSideEffects
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat( DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}