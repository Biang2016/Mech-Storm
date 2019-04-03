using System.Collections.Generic;

public class DrawCards_Base : CardDeckRelatedSideEffects, IEffectFactor
{
    public SideEffectValue Value = new SideEffectValue(0);
    private int factor = 1;

    public List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value}; }
    }

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


    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((DrawCards_Base) copy).Value = Value.Clone();
        ((DrawCards_Base) copy).SetFactor(GetFactor());
    }
}