public class AddLifeOne_Base : TargetSideEffect, IEffectFactor
{
    public int Value;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }

    }

    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, ((M_TargetRange == TargetRange.SelfShip || M_TargetRange == TargetRange.EnemyShip) ? "" : "一个") + GetChineseDescOfTargetRange(M_TargetRange), FinalValue);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        Value = reader.ReadSInt32();
    }

    public override int CalculateDamage()
    {
        return 0;
    }

    public override int CalculateHeal()
    {
        return 0;
    }

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddLifeOne_Base) copy).Value = Value;
        ((AddLifeOne_Base) copy).Factor = Factor;
    }
}