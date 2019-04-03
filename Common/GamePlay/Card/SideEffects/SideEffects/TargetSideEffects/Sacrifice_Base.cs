using System.Collections.Generic;

public class Sacrifice_Base : TargetSideEffect, IEffectFactor, IDamage
{
    public List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {ValueBasic, ValuePlus}; }
    }

    public SideEffectValue ValueBasic = new SideEffectValue(0);
    public SideEffectValue ValuePlus = new SideEffectValue(0);
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
        get { return ValueBasic.Value * GetFactor(); }
    }

    public int FinalValuePlus
    {
        get { return ValuePlus.Value * GetFactor(); }
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,false, false), FinalValueBasic, FinalValuePlus);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ValueBasic.Value);
        writer.WriteSInt32(ValuePlus.Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ValueBasic.Value = reader.ReadSInt32();
        ValuePlus.Value = reader.ReadSInt32();
    }

    public int CalculateDamage()
    {
        return 0;
    }


    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((Sacrifice_Base) copy).ValueBasic = ValueBasic.Clone();
        ((Sacrifice_Base) copy).ValuePlus = ValuePlus.Clone();
        ((Sacrifice_Base) copy).SetFactor(GetFactor());
    }
}