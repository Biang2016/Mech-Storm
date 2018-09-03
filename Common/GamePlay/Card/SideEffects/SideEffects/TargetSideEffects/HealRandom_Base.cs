public class HealRandom_Base : TargetSideEffect
{
    public int Value;

    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, "一个随机" + GetChineseDescOfTargetRange(M_TargetRange), Value);
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

    public override int CalculateDamage()
    {
        return 0;
    }

    public override int CalculateHeal()
    {
        return Value;
    }
}