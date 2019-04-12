using System.Collections.Generic;

public class AddSelfWeaponEnergy_Base : AttachedEquipSideEffects, IEffectFactor
{
    public SideEffectValue Value = new SideEffectValue(0);

    public override List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value}; }
    }

    private int factor = 1;
    public int RetinueID;

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
        ((AddSelfWeaponEnergy_Base) copy).RetinueID = RetinueID;
        ((AddSelfWeaponEnergy_Base) copy).Value = Value.Clone();
        ((AddSelfWeaponEnergy_Base) copy).SetFactor(GetFactor());
    }
}