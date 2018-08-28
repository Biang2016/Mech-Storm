public class DrawCards_Base : SideEffectBase
{
    public int Value;

    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaw, Value);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        Value = reader.ReadSInt32();
    }
}