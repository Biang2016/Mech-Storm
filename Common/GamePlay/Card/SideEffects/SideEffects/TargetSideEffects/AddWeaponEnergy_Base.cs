using System.Collections.Generic;

public class AddWeaponEnergy_Base : TargetSideEffect, IEffectFactor
{
    public int RetinueID;
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


    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), FinalValue);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(RetinueID);
        writer.WriteSInt32(Value.Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        RetinueID = reader.ReadSInt32();
        Value.Value = reader.ReadSInt32();
    }


    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddWeaponEnergy_Base) copy).RetinueID = RetinueID;
        ((AddWeaponEnergy_Base) copy).Value = Value.Clone();
        ((AddWeaponEnergy_Base) copy).SetFactor(GetFactor());
    }
}