public class GetHandCardCopy_Base : CardRelatedSideEffect
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
    }

}