public class WaitInHandDecreaseMagic_Base : CardRelatedSideEffect,IEffectFactor
{
    public int Value;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }
        set { Value = value; }
    }

    public int CardInstanceID;

    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, FinalValue);
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

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }
}