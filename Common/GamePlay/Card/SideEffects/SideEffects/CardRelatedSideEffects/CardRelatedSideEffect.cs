public abstract class CardRelatedSideEffect : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_Bool("IsNeedChoise", false);
    }

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