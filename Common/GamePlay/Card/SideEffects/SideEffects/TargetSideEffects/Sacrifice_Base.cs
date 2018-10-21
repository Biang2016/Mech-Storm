public class Sacrifice_Base : TargetSideEffect, IEffectFactor, IDamage
{
    public int ValueBasic;
    public int ValuePlus;
    private int factor = 1;

    public int GetFactor()
    {
        return factor;
    }

    public void SetFactor(int value)
    {
        factor = value;
    }

    public int FinalValueBasic
    {
        get { return ValueBasic * GetFactor(); }
    }

    public int FinalValuePlus
    {
        get { return ValuePlus * GetFactor(); }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), FinalValueBasic, FinalValuePlus);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ValueBasic);
        writer.WriteSInt32(ValuePlus);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ValueBasic = reader.ReadSInt32();
        ValuePlus = reader.ReadSInt32();
    }

    public int CalculateDamage()
    {
        return 0;
    }


    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((Sacrifice_Base) copy).ValueBasic = ValueBasic;
        ((Sacrifice_Base) copy).ValuePlus = ValuePlus;
        ((Sacrifice_Base) copy).SetFactor(GetFactor());
    }
}