public class GetHandCardCopy_Base : CardRelatedSideEffect
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }

}