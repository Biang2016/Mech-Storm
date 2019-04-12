using System.Collections.Generic;

public class WaitInHandDecreaseEnergy_Base : CardRelatedSideEffect, IEffectFactor
{
    public override List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value}; }
    }

    private int factor = 1;

    public SideEffectValue Value = new SideEffectValue(0);

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
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], FinalValue);
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

    public void SetEffetFactor(int factor)
    {
        SetFactor(factor);
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((WaitInHandDecreaseEnergy_Base) copy).Value = Value.Clone();
        ((WaitInHandDecreaseEnergy_Base) copy).SetFactor(GetFactor());
    }
}