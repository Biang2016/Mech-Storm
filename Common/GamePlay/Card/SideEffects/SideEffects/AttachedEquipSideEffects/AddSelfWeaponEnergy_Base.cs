public class AddSelfWeaponEnergy_Base : AttachedEquipSideEffects, IEffectFactor
{
    public int RetinueID;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }
    }

    public int Value;

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, FinalValue);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(RetinueID);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        RetinueID = reader.ReadSInt32();
        Value = reader.ReadSInt32();
    }

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddSelfWeaponEnergy_Base) copy).RetinueID = RetinueID;
        ((AddSelfWeaponEnergy_Base) copy).Value = Value;
        ((AddSelfWeaponEnergy_Base) copy).Factor = Factor;
    }
}