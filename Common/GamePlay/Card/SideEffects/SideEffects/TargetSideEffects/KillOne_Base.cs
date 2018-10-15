public class KillOne_Base : TargetSideEffect
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat( isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false));
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
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