public class GetHandCardCopy_Base : CardRelatedSideEffect
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
    }
}