using System;

public abstract class CardRelatedSideEffect : SideEffectBase
{
    public bool IsNeedChoise;
    public int TargetCardInstanceId;

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(TargetCardInstanceId);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        TargetCardInstanceId = reader.ReadSInt32();
    }
}