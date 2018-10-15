public class AddWeaponEnergy_Base : TargetSideEffect, IEffectFactor
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
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), FinalValue);
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

    public override int CalculateDamage()
    {
        return 0;
    }

    public override int CalculateHeal()
    {
        return 0;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddWeaponEnergy_Base) copy).RetinueID = RetinueID;
        ((AddWeaponEnergy_Base) copy).Value = Value;
        ((AddWeaponEnergy_Base) copy).Factor = Factor;
    }
}