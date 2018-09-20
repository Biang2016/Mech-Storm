public class AddWeaponEnergy_Base : SideEffectBase, IEffectFactor
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
        return HightlightStringFormat(HightlightColor, isEnglish ? DescRaw_en : DescRaw, FinalValue);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(RetinueID);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
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
        ((AddWeaponEnergy_Base) copy).RetinueID = RetinueID;
        ((AddWeaponEnergy_Base) copy).Value = Value;
        ((AddWeaponEnergy_Base) copy).Factor = Factor;
    }
}