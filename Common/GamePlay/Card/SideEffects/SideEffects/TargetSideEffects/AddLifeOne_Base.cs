public class AddLifeOne_Base : TargetSideEffect
{
    public int Value;

    public override string GenerateDesc()
    {
        return HightlightStringFormat(HightlightColor, DescRaw, ((M_TargetRange == TargetRange.SelfShip || M_TargetRange == TargetRange.EnemyShip) ? "" : "一个") + GetChineseDescOfTargetRange(M_TargetRange), Value);
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
}