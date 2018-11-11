using System.Collections.Generic;

public class UseAllEnergyDamageShip_Base : SideEffectBase, IEffectFactor, IDamage
{
    public List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value, Value_Plus}; }
    }

    public SideEffectValue Value = new SideEffectValue(0);
    public SideEffectValue Value_Plus = new SideEffectValue(0);
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

    public int FinalValue_Plus
    {
        get { return Value_Plus.Value * GetFactor(); }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, FinalValue, FinalValue_Plus);
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value.Value);
        writer.WriteSInt32(Value_Plus.Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value.Value = reader.ReadSInt32();
        Value_Plus.Value = reader.ReadSInt32();
    }

    public int CalculateDamage()
    {
        return FinalValue;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((UseAllEnergyDamageShip_Base) copy).Value = Value.Clone();
        ((UseAllEnergyDamageShip_Base) copy).Value_Plus = Value_Plus;
        ((UseAllEnergyDamageShip_Base) copy).SetFactor(GetFactor());
    }
}