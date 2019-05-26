public abstract class HandCardRelatedSideEffect : SideEffectBase
{
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
}