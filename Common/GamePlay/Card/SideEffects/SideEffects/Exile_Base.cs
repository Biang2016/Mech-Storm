public class Exile_Base : SideEffectBase
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw);
    }
}