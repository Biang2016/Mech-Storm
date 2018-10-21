public class NextSpellDamageDouble_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat( isEnglish ? DescRaw_en : DescRaw);
    }
}