public class HealOne_Base : TargetSideEffect
{
    public int Value;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(HightlightColor, isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), FinalValue);
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
        return FinalValue;
    }

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((HealOne_Base) copy).Value = Value;
        ((HealOne_Base) copy).Factor = Factor;
    }
}