public class GetHandCardCopy_Base : SideEffectBase
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
    }
}