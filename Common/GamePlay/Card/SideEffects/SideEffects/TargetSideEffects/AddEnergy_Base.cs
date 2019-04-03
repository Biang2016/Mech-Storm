using System.Collections.Generic;

public class AddEnergy_Base : TargetSideEffect, IEffectFactor
{
    public SideEffectValue Value = new SideEffectValue(0);

    public List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value}; }
    }

    private int factor = 1;

    public int GetFactor()
    {
        return factor;
    }

    public void SetFactor(int value)
    {
        factor = value;
    }

    public int FinalValue
    {
        get { return Value.Value * GetFactor(); }
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,false, false), FinalValue);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value.Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value.Value = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddEnergy_Base) copy).Value = Value.Clone();
        ((AddEnergy_Base) copy).SetFactor(GetFactor());
    }
}