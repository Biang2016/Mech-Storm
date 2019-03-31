public class SummonRetinuePerMechs_Base : TargetSideEffect
{
    public int RetinueCardId;

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(RetinueCardId).BaseInfo;
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange,false, false), bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
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