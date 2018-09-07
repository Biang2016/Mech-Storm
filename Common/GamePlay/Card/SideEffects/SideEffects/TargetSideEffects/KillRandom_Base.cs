public class KillRandom_Base : TargetSideEffect
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, "一个随机" + GetChineseDescOfTargetRange(M_TargetRange));
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

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
    }
}