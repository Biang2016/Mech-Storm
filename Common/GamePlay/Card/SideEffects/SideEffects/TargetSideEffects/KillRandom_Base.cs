public class KillRandom_Base : TargetSideEffect
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaw, GetChineseDescOfTargetRange(M_TargetRange));
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
    }

    public override int CalculateDamage()
    {
        return 0;
    }

    public override int CalculateHeal()
    {
        return 0;
    }
}