public class AddEnergy_Base : SideEffectBase
{
    public int RetinueID;
    public int Value;

    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, Value);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(RetinueID);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        RetinueID = reader.ReadSInt32();
        Value = reader.ReadSInt32();
    }
}