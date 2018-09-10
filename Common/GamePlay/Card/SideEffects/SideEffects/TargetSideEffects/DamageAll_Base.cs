public class DamageAll_Base : TargetSideEffect
{
    public int Value;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(HightlightColor, isEnglish ? DescRaw_en : DescRaw, (isEnglish ? "all " : "所有") + GetChineseDescOfTargetRange(M_TargetRange, isEnglish), FinalValue);
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
        return FinalValue;
    }

    public override int CalculateHeal()
    {
        return 0;
    }


    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }


    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((DamageAll_Base) copy).Value = Value;
        ((DamageAll_Base) copy).Factor = Factor;
    }
}