public class SummonRetinuePerMechs_Base : TargetSideEffect
{
    public int RetinueCardId;

    public override string GenerateDesc(bool isEnglish)
    {
        BaseInfo bi = AllCards.GetCard(RetinueCardId).BaseInfo;
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), isEnglish ? bi.CardName_en : bi.CardName);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(RetinueCardId);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        RetinueCardId = reader.ReadSInt32();
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
        ((SummonRetinuePerMechs_Base) copy).RetinueCardId = RetinueCardId;
    }
}