public class Sacrifice_Base : TargetSideEffect, IEffectFactor
{
    public int ValueBasic;
    public int ValuePlus;
    public int Factor = 1;

    public int FinalValueBasic
    {
        get { return ValueBasic * Factor; }
    }

    public int FinalValuePlus
    {
        get { return ValuePlus * Factor; }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), FinalValueBasic, FinalValuePlus);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(ValueBasic);
        writer.WriteSInt32(ValuePlus);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        ValueBasic = reader.ReadSInt32();
        ValuePlus = reader.ReadSInt32();
    }

    public override int CalculateDamage()
    {
        return 0;
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
        ((Sacrifice_Base) copy).ValueBasic = ValueBasic;
        ((Sacrifice_Base) copy).ValuePlus = ValuePlus;
        ((Sacrifice_Base) copy).Factor = Factor;
    }
}