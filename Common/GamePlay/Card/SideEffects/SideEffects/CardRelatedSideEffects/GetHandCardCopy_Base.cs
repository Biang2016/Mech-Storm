public class GetHandCardCopy_Base : CardRelatedSideEffect
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}