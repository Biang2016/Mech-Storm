public class GetHandCardCopy_Base : HandCardRelatedSideEffect
{
    protected override void InitSideEffectParam()
    {
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }
}