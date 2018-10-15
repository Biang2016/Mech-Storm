public abstract class CardRelatedSideEffect : SideEffectBase
{
    public bool IsNeedChoise;
    public int TargetCardInstanceId;

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(TargetCardInstanceId);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        TargetCardInstanceId = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((CardRelatedSideEffect) copy).IsNeedChoise = IsNeedChoise;
        ((CardRelatedSideEffect) copy).TargetCardInstanceId = TargetCardInstanceId;
    }
}