public class Exile_Base : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}