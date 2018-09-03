public class DamageAll_Base : TargetSideEffect
{
    public int Value;

    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, "所有" + GetChineseDescOfTargetRange(M_TargetRange), Value);
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
        return Value;
    }

    public override int CalculateHeal()
    {
        return 0;
    }
}