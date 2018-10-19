public class GetHandCardCopy_Base : CardRelatedSideEffect
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw);
    }

}