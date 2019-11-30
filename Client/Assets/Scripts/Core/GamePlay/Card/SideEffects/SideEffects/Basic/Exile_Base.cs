public class Exile_Base : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
    }

    public override string GenerateDesc()
    {
        return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}