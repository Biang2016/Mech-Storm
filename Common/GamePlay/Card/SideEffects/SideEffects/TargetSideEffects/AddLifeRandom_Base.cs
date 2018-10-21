public class AddLifeRandom_Base : TargetSideEffect, IEffectFactor
{
    public int Value;
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
        get { return Value * GetFactor(); }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat( isEnglish ? DescRaw_en : DescRaw,  GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, true), FinalValue);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddLifeRandom_Base) copy).Value = Value;
        ((AddLifeRandom_Base) copy).SetFactor(GetFactor());
    }
}