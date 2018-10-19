public class SummonRetinuePerMechs_Base : TargetSideEffect
{
    public int RetinueCardId;

    public override string GenerateDesc(bool isEnglish)
    {
        BaseInfo bi = AllCards.GetCard(RetinueCardId).BaseInfo;
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), isEnglish ? bi.CardName_en : bi.CardName);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(RetinueCardId);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        RetinueCardId = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((SummonRetinuePerMechs_Base) copy).RetinueCardId = RetinueCardId;
    }
}